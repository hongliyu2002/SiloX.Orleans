using System.Collections.Concurrent;
using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using SiloX.Domain.Abstractions;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;
using Vending.Domain.Abstractions.Snacks;
using Vending.Domain.EntityFrameworkCore;
using Vending.Projection.Abstractions.Machines;
using Vending.Projection.EntityFrameworkCore;

namespace Vending.Synchronizer.Machines;

/// <summary>
///     Grain implementation class MachineSynchronizerGrain.
/// </summary>
public class MachineSynchronizerGrain : PublisherGrainWithGuidKey<MachineInfoEvent, MachineInfoErrorEvent>, IMachineSynchronizerGrain
{
    private readonly ProjectionDbContext _projectionDbContext;
    private readonly DomainDbContext _domainDbContext;
    private readonly ILogger<MachineSynchronizerGrain> _logger;

    private IGrainReminder? _syncDifferencesReminder;
    private IGrainReminder? _syncAllReminder;

    /// <inheritdoc />
    public MachineSynchronizerGrain(ProjectionDbContext projectionDbContext, DomainDbContext domainDbContext, ILogger<MachineSynchronizerGrain> logger)
        : base(Constants.StreamProviderName)
    {
        _projectionDbContext = Guard.Against.Null(projectionDbContext, nameof(projectionDbContext));
        _domainDbContext = Guard.Against.Null(domainDbContext, nameof(domainDbContext));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    protected override string GetPubStreamNamespace()
    {
        return Constants.MachineInfosNamespace;
    }

    /// <inheritdoc />
    protected override string GetPubBroadcastStreamNamespace()
    {
        return Constants.MachineInfosBroadcastNamespace;
    }

    /// <inheritdoc />
    public async Task ReceiveReminder(string reminderName, TickStatus status)
    {
        switch (reminderName)
        {
            case Constants.MachineInfosSyncDifferencesReminderName:
                await SyncDifferencesAsync();
                return;
            case Constants.MachineInfosSyncAllReminderName:
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
            case Constants.MachineInfosSyncDifferencesReminderName:
                _syncDifferencesReminder = await this.RegisterOrUpdateReminder(reminderName, dueTime, period);
                return;
            case Constants.MachineInfosSyncAllReminderName:
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
            case Constants.MachineInfosSyncDifferencesReminderName:
                _syncDifferencesReminder ??= await this.GetReminder(reminderName);
                if (_syncDifferencesReminder != null)
                {
                    await this.UnregisterReminder(_syncDifferencesReminder);
                    _syncDifferencesReminder = null;
                }
                return;
            case Constants.MachineInfosSyncAllReminderName:
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
    public async Task SyncAsync(Guid machineId)
    {
        try
        {
            var machine = await _domainDbContext.Machines.FindAsync(machineId);
            var machineInfo = await _projectionDbContext.Machines.FindAsync(machineId);
            if (machine == null)
            {
                if (machineInfo == null)
                {
                    return;
                }
                _projectionDbContext.Remove(machineInfo);
                await _projectionDbContext.SaveChangesAsync();
                return;
            }
            await ApplyFullUpdateAsync(machineId);
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
            var machineIdVersions = await _domainDbContext.Machines.Select(s => new { s.Id, Version = EF.Property<int>(s, "Version") }).ToDictionaryAsync(r => r.Id, r => r.Version);
            var machineInfoIdVersions = await _projectionDbContext.Machines.Select(s => new { s.Id, s.Version }).ToDictionaryAsync(r => r.Id, r => r.Version);
            var machineInfoIdVersionsToRemove = machineInfoIdVersions.Except(machineIdVersions, true);
            var machineInfoIdsToRemove = machineInfoIdVersionsToRemove.Keys.ToArray();
            var machineInfosToRemove = await _projectionDbContext.Machines.Where(s => machineInfoIdsToRemove.Contains(s.Id)).ToListAsync();
            if (machineInfosToRemove is { Count: > 0 })
            {
                _projectionDbContext.RemoveRange(machineInfosToRemove);
                await _projectionDbContext.SaveChangesAsync();
            }
            var machineIdVersionsToSync = machineIdVersions.Except(machineInfoIdVersions);
            foreach (var machineIdVersion in machineIdVersionsToSync)
            {
                await ApplyFullUpdateAsync(machineIdVersion.Key);
            }
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
            var machineIdVersions = await _domainDbContext.Machines.Select(s => new { s.Id, Version = EF.Property<int>(s, "Version") }).ToDictionaryAsync(r => r.Id, r => r.Version);
            var machineInfoIdVersions = await _projectionDbContext.Machines.Select(s => new { s.Id, s.Version }).ToDictionaryAsync(r => r.Id, r => r.Version);
            var machineInfoIdVersionsToRemove = machineInfoIdVersions.Except(machineIdVersions, true);
            var machineInfoIdsToRemove = machineInfoIdVersionsToRemove.Keys.ToArray();
            var machineInfosToRemove = await _projectionDbContext.Machines.Where(s => machineInfoIdsToRemove.Contains(s.Id)).ToListAsync();
            if (machineInfosToRemove is { Count: > 0 })
            {
                _projectionDbContext.RemoveRange(machineInfosToRemove);
                await _projectionDbContext.SaveChangesAsync();
            }
            var machineIdVersionsToSync = machineIdVersions;
            foreach (var machineIdVersion in machineIdVersionsToSync)
            {
                await ApplyFullUpdateAsync(machineIdVersion.Key);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SyncDifferencesAsync: Exception is occurred when try to write data to the database. Try to execute full update...");
        }
    }

    private async Task ApplyFullUpdateAsync(Guid machineId)
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
                var machineGrain = GrainFactory.GetGrain<IMachineGrain>(machineId);
                var machine = await machineGrain.GetMachineAsync();
                var machineInfo = await _projectionDbContext.Machines.Include(m => m.Slots).FirstOrDefaultAsync(m => m.Id == machineId);
                if (machine == null || machine.Id == Guid.Empty)
                {
                    if (machineInfo == null)
                    {
                        return;
                    }
                    _projectionDbContext.Remove(machineInfo);
                    await _projectionDbContext.SaveChangesAsync();
                    return;
                }
                if (machineInfo == null)
                {
                    machineInfo = new MachineInfo();
                    _projectionDbContext.Machines.Add(machineInfo);
                }
                machineInfo = await machine.ToProjection(GetSnackNameAndPictureUrlAsync, machineInfo);
                machineInfo.Version = await machineGrain.GetVersionAsync();
                var statsOfPurchasesGrain = GrainFactory.GetGrain<IMachineStatsOfPurchasesGrain>(machineId);
                machineInfo.BoughtCount = await statsOfPurchasesGrain.GetBoughtCountAsync();
                machineInfo.BoughtAmount = await statsOfPurchasesGrain.GetBoughtAmountAsync();
                var changes = await _projectionDbContext.SaveChangesAsync();
                if (changes > 0)
                {
                    await PublishAsync(new MachineInfoSavedEvent(machineInfo.Id, machineInfo.Version, machineInfo, traceId, operatedAt, operatedBy));
                }
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
                await PublishErrorAsync(new MachineInfoErrorEvent(machineId, -1, 20, new[] { ex.Message }, traceId, operatedAt, operatedBy));
                retryNeeded = false;
            }
        }
        while (retryNeeded);
    }

    #region Get SnackInfo Name And Picture Url

    private readonly ConcurrentDictionary<Guid, (string SnackName, string? SnackPictureUrl)> _snackNamePictureCache = new();

    private async Task<(string SnackName, string? SnackPictureUrl)> GetSnackNameAndPictureUrlAsync(Guid snackId)
    {
        if (_snackNamePictureCache.TryGetValue(snackId, out var snackNamePicture))
        {
            return snackNamePicture;
        }
        var snackGrain = GrainFactory.GetGrain<ISnackGrain>(snackId);
        var snackInGrain = await snackGrain.GetSnackAsync();
        snackNamePicture = (snackInGrain.Name, snackInGrain.PictureUrl);
        _snackNamePictureCache.TryAdd(snackId, snackNamePicture);
        return snackNamePicture;
    }

    #endregion

}