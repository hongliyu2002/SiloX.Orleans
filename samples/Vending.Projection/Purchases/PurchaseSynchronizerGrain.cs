using System.Collections.Concurrent;
using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Purchases;
using Vending.Domain.Abstractions.Snacks;
using Vending.Domain.EntityFrameworkCore;
using Vending.Projection.Abstractions.Purchases;
using Vending.Projection.EntityFrameworkCore;

namespace Vending.Synchronizer.Purchases;

/// <summary>
///     Grain implementation class PurchaseSynchronizerGrain.
/// </summary>
public class PurchaseSynchronizerGrain : PublisherGrainWithGuidKey<PurchaseInfoEvent, PurchaseInfoErrorEvent>, IPurchaseSynchronizerGrain
{
    private readonly ProjectionDbContext _projectionDbContext;
    private readonly DomainDbContext _domainDbContext;
    private readonly ILogger<PurchaseSynchronizerGrain> _logger;

    private IGrainReminder? _syncDifferencesReminder;
    private IGrainReminder? _syncAllReminder;

    /// <inheritdoc />
    public PurchaseSynchronizerGrain(ProjectionDbContext projectionDbContext, DomainDbContext domainDbContext, ILogger<PurchaseSynchronizerGrain> logger)
        : base(Constants.StreamProviderName)
    {
        _projectionDbContext = Guard.Against.Null(projectionDbContext, nameof(projectionDbContext));
        _domainDbContext = Guard.Against.Null(domainDbContext, nameof(domainDbContext));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    protected override string GetPubStreamNamespace()
    {
        return Constants.PurchaseInfosNamespace;
    }

    /// <inheritdoc />
    protected override string GetPubBroadcastStreamNamespace()
    {
        return Constants.PurchaseInfosBroadcastNamespace;
    }

    /// <inheritdoc />
    public async Task ReceiveReminder(string reminderName, TickStatus status)
    {
        switch (reminderName)
        {
            case Constants.PurchaseInfosSyncDifferencesReminderName:
                await SyncDifferencesAsync();
                return;
            case Constants.PurchaseInfosSyncAllReminderName:
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
            case Constants.PurchaseInfosSyncDifferencesReminderName:
                _syncDifferencesReminder = await this.RegisterOrUpdateReminder(reminderName, dueTime, period);
                return;
            case Constants.PurchaseInfosSyncAllReminderName:
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
            case Constants.PurchaseInfosSyncDifferencesReminderName:
                _syncDifferencesReminder ??= await this.GetReminder(reminderName);
                if (_syncDifferencesReminder != null)
                {
                    await this.UnregisterReminder(_syncDifferencesReminder);
                    _syncDifferencesReminder = null;
                }
                return;
            case Constants.PurchaseInfosSyncAllReminderName:
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
    public async Task SyncAsync(Guid purchaseId)
    {
        try
        {
            var purchase = await _domainDbContext.Purchases.FindAsync(purchaseId);
            var purchaseInfo = await _projectionDbContext.Purchases.FindAsync(purchaseId);
            if (purchase == null)
            {
                if (purchaseInfo == null)
                {
                    return;
                }
                _projectionDbContext.Remove(purchaseInfo);
                await _projectionDbContext.SaveChangesAsync();
                return;
            }
            await ApplyFullUpdateAsync(purchaseId);
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
            var purchaseIds = await _domainDbContext.Purchases.Select(s => s.Id).ToListAsync();
            var purchaseInfoIds = await _projectionDbContext.Purchases.Select(s => s.Id).ToListAsync();
            var purchaseInfoIdsToRemove = purchaseInfoIds.Except(purchaseIds);
            var purchaseInfosToRemove = await _projectionDbContext.Purchases.Where(s => purchaseInfoIdsToRemove.Contains(s.Id)).ToListAsync();
            if (purchaseInfosToRemove is { Count: > 0 })
            {
                _projectionDbContext.RemoveRange(purchaseInfosToRemove);
                await _projectionDbContext.SaveChangesAsync();
            }
            var purchaseIdsToSync = purchaseIds.Except(purchaseInfoIds);
            await Task.WhenAll(purchaseIdsToSync.Select(ApplyFullUpdateAsync));
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
            var purchaseIds = await _domainDbContext.Purchases.Select(s => s.Id).ToListAsync();
            var purchaseInfoIds = await _projectionDbContext.Purchases.Select(s => s.Id).ToListAsync();
            var purchaseInfoIdsToRemove = purchaseInfoIds.Except(purchaseIds);
            var purchaseInfosToRemove = await _projectionDbContext.Purchases.Where(s => purchaseInfoIdsToRemove.Contains(s.Id)).ToListAsync();
            if (purchaseInfosToRemove is { Count: > 0 })
            {
                _projectionDbContext.RemoveRange(purchaseInfosToRemove);
                await _projectionDbContext.SaveChangesAsync();
            }
            var purchaseIdsToSync = purchaseIds;
            await Task.WhenAll(purchaseIdsToSync.Select(ApplyFullUpdateAsync));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SyncDifferencesAsync: Exception is occurred when try to write data to the database. Try to execute full update...");
        }
    }

    private async Task ApplyFullUpdateAsync(Guid purchaseId)
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
                var purchaseGrain = GrainFactory.GetGrain<IPurchaseGrain>(purchaseId);
                var purchase = await purchaseGrain.GetPurchaseAsync();
                var purchaseInfo = await _projectionDbContext.Purchases.FindAsync(purchaseId);
                if (purchase == null || purchase.Id == Guid.Empty)
                {
                    if (purchaseInfo == null)
                    {
                        return;
                    }
                    _projectionDbContext.Remove(purchaseInfo);
                    await _projectionDbContext.SaveChangesAsync();
                    return;
                }
                if (purchaseInfo == null)
                {
                    purchaseInfo = new PurchaseInfo();
                    _projectionDbContext.Purchases.Add(purchaseInfo);
                }
                await purchase.ToProjection(GetSnackNameAndPictureUrlAsync, purchaseInfo);
                var changes = await _projectionDbContext.SaveChangesAsync();
                if (changes > 0)
                {
                    await PublishAsync(new PurchaseInfoSavedEvent(purchaseInfo.Id, purchaseInfo.Version, purchaseInfo, traceId, operatedAt, operatedBy));
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
                await PublishErrorAsync(new PurchaseInfoErrorEvent(purchaseId, -1, 30, new[] { ex.Message }, traceId, operatedAt, operatedBy));
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