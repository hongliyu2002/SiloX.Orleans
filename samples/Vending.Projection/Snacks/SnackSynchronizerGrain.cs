﻿using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Snacks;
using Vending.Projection.Abstractions.Snacks;
using Vending.Projection.EntityFrameworkCore;

namespace Vending.Synchronizer.Snacks;

/// <summary>
///     Grain implementation class SnackSynchronizerGrain.
/// </summary>
public class SnackSynchronizerGrain : PublisherGrainWithGuidKey<SnackInfoEvent, SnackInfoErrorEvent>, ISnackSynchronizerGrain
{
    private readonly ProjectionDbContext _projectionDbContext;
    private readonly ProjectionDbContext _domainDbContext;
    private readonly ILogger<SnackSynchronizerGrain> _logger;

    private IGrainReminder? _syncDifferencesReminder;
    private IGrainReminder? _syncAllReminder;

    /// <inheritdoc />
    public SnackSynchronizerGrain(ProjectionDbContext projectionDbContext, ProjectionDbContext domainDbContext, ILogger<SnackSynchronizerGrain> logger)
        : base(Constants.StreamProviderName)
    {
        _projectionDbContext = Guard.Against.Null(projectionDbContext, nameof(projectionDbContext));
        _domainDbContext = Guard.Against.Null(domainDbContext, nameof(domainDbContext));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    protected override string GetPubStreamNamespace()
    {
        return Constants.SnackInfosNamespace;
    }

    /// <inheritdoc />
    protected override string GetPubBroadcastStreamNamespace()
    {
        return Constants.SnackInfosBroadcastNamespace;
    }

    /// <inheritdoc />
    public async Task ReceiveReminder(string reminderName, TickStatus status)
    {
        switch (reminderName)
        {
            case Constants.SnackInfosSyncDifferencesReminderName:
                await SyncDifferencesAsync();
                return;
            case Constants.SnackInfosSyncAllReminderName:
                await SyncAllAsync();
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(reminderName), reminderName, null);
        }
    }

    /// <inheritdoc />
    public async Task StartReminder(string reminderName, TimeSpan dueTime, TimeSpan period)
    {
        switch (reminderName)
        {
            case Constants.SnackInfosSyncDifferencesReminderName:
                _syncDifferencesReminder = await this.RegisterOrUpdateReminder(reminderName, dueTime, period);
                return;
            case Constants.SnackInfosSyncAllReminderName:
                _syncAllReminder = await this.RegisterOrUpdateReminder(reminderName, dueTime, period);
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(reminderName), reminderName, null);
        }
    }

    /// <inheritdoc />
    public async Task StopReminder(string reminderName)
    {
        switch (reminderName)
        {
            case Constants.SnackInfosSyncDifferencesReminderName:
                _syncDifferencesReminder ??= await this.GetReminder(reminderName);
                if (_syncDifferencesReminder != null)
                {
                    await this.UnregisterReminder(_syncDifferencesReminder);
                    _syncDifferencesReminder = null;
                }
                return;
            case Constants.SnackInfosSyncAllReminderName:
                _syncAllReminder ??= await this.GetReminder(reminderName);
                if (_syncAllReminder != null)
                {
                    await this.UnregisterReminder(_syncAllReminder);
                    _syncAllReminder = null;
                }
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(reminderName), reminderName, null);
        }
    }

    /// <inheritdoc />
    public async Task SyncAsync(Guid snackId)
    {
        try
        {
            var snack = await _domainDbContext.Snacks.FindAsync(snackId);
            var snackInfo = await _projectionDbContext.Snacks.FindAsync(snackId);
            if (snack == null)
            {
                if (snackInfo == null)
                {
                    return;
                }
                _projectionDbContext.Remove(snackInfo);
                await _projectionDbContext.SaveChangesAsync();
                return;
            }
            await ApplyFullUpdateAsync(snackId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SyncAsync: Exception is occurred when try to write data to the database. Try to execute full update...");
        }
    }

    /// <inheritdoc />
    public async Task SyncDifferencesAsync()
    {
        try
        {
            var snackIds = await _domainDbContext.Snacks.Select(s => s.Id).ToListAsync();
            var snackInfoIds = await _projectionDbContext.Snacks.Select(s => s.Id).ToListAsync();
            var snackInfoIdsToRemove = snackInfoIds.Except(snackIds);
            var snackInfosToRemove = await _projectionDbContext.Snacks.Where(s => snackInfoIdsToRemove.Contains(s.Id)).ToListAsync();
            if (snackInfosToRemove is { Count: > 0 })
            {
                _projectionDbContext.RemoveRange(snackInfosToRemove);
                await _projectionDbContext.SaveChangesAsync();
            }
            var snackIdsToSync = snackIds.Except(snackInfoIds);
            await Task.WhenAll(snackIdsToSync.Select(ApplyFullUpdateAsync));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SyncDifferencesAsync: Exception is occurred when try to write data to the database. Try to execute full update...");
        }
    }

    /// <inheritdoc />
    public async Task SyncAllAsync()
    {
        try
        {
            var snackIds = await _domainDbContext.Snacks.Select(s => s.Id).ToListAsync();
            var snackInfoIds = await _projectionDbContext.Snacks.Select(s => s.Id).ToListAsync();
            var snackInfoIdsToRemove = snackInfoIds.Except(snackIds);
            var snackInfosToRemove = await _projectionDbContext.Snacks.Where(s => snackInfoIdsToRemove.Contains(s.Id)).ToListAsync();
            if (snackInfosToRemove is { Count: > 0 })
            {
                _projectionDbContext.RemoveRange(snackInfosToRemove);
                await _projectionDbContext.SaveChangesAsync();
            }
            var snackIdsToSync = snackIds;
            await Task.WhenAll(snackIdsToSync.Select(ApplyFullUpdateAsync));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SyncDifferencesAsync: Exception is occurred when try to write data to the database. Try to execute full update...");
        }
    }

    private async Task ApplyFullUpdateAsync(Guid snackId)
    {
        var attempts = 0;
        bool retryNeeded;
        do
        {
            var traceId = Guid.NewGuid();
            var operatedAt = DateTimeOffset.UtcNow;
            var operatedBy = $"System/{GetType().Name}";
            try
            {
                var snackGrain = GrainFactory.GetGrain<ISnackGrain>(snackId);
                var snack = await snackGrain.GetSnackAsync();
                var snackInfo = await _projectionDbContext.Snacks.FindAsync(snackId);
                if (snack == null || snack.Id == Guid.Empty)
                {
                    if (snackInfo == null)
                    {
                        return;
                    }
                    _projectionDbContext.Remove(snackInfo);
                    await _projectionDbContext.SaveChangesAsync();
                    return;
                }
                if (snackInfo == null)
                {
                    snackInfo = new SnackInfo();
                    _projectionDbContext.Snacks.Add(snackInfo);
                }
                snackInfo = snack.ToProjection(snackInfo);
                snackInfo.Version = await snackGrain.GetVersionAsync();
                var statsOfMachinesGrain = GrainFactory.GetGrain<ISnackStatsOfMachinesGrain>(snackId);
                snackInfo.MachineCount = await statsOfMachinesGrain.GetMachineCountAsync();
                snackInfo.TotalQuantity = await statsOfMachinesGrain.GetTotalQuantityAsync();
                snackInfo.TotalAmount = await statsOfMachinesGrain.GetTotalAmountAsync();
                var statsOfPurchasesGrain = GrainFactory.GetGrain<ISnackStatsOfPurchasesGrain>(snackId);
                snackInfo.BoughtCount = await statsOfPurchasesGrain.GetBoughtCountAsync();
                snackInfo.BoughtAmount = await statsOfPurchasesGrain.GetBoughtAmountAsync();
                await _projectionDbContext.SaveChangesAsync();
                await PublishAsync(new SnackInfoSavedEvent(snackInfo.Id, snackInfo.Version, snackInfo, traceId, operatedAt, operatedBy));
                return;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                retryNeeded = ++attempts <= 3;
                if (retryNeeded)
                {
                    _logger.LogWarning(ex, "ApplyFullUpdateAsync: DbUpdateConcurrencyException is occurred when try to write data to the database. Retrying {Attempts}...", attempts);
                    await Task.Delay(TimeSpan.FromSeconds(attempts));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ApplyFullUpdateAsync: Exception is occurred when try to write data to the database");
                await PublishErrorAsync(new SnackInfoErrorEvent(snackId, -1, 10, new[] { ex.Message }, traceId, operatedAt, operatedBy));
                retryNeeded = false;
            }
        }
        while (retryNeeded);
    }
}