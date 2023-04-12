using System.Collections.Concurrent;
using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Purchases;
using Vending.Domain.Abstractions.Snacks;
using Vending.Projection.Abstractions.Purchases;
using Vending.Projection.EntityFrameworkCore;

namespace Vending.Projection.Purchases;

[ImplicitStreamSubscription(Constants.PurchasesNamespace)]
public sealed class PurchaseProjectionGrain : SubscriberPublisherGrainWithGuidKey<PurchaseEvent, PurchaseErrorEvent, PurchaseInfoEvent, PurchaseInfoErrorEvent>, IPurchaseProjectionGrain
{
    private readonly ProjectionDbContext _dbContext;
    private readonly ILogger<PurchaseProjectionGrain> _logger;

    /// <inheritdoc />
    public PurchaseProjectionGrain(ProjectionDbContext dbContext, ILogger<PurchaseProjectionGrain> logger)
        : base(Constants.StreamProviderName)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    protected override string GetSubStreamNamespace()
    {
        return Constants.PurchasesNamespace;
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
    protected override Task HandLeEventAsync(PurchaseEvent domainEvent)
    {
        return domainEvent switch
               {
                   PurchaseInitializedEvent purchaseEvent => ApplyEventAsync(purchaseEvent),
                   _ => Task.CompletedTask
               };
    }

    /// <inheritdoc />
    protected override Task HandLeErrorEventAsync(PurchaseErrorEvent errorEvent)
    {
        _logger.LogWarning("PurchaseErrorEvent received: {Reasons}", string.Join(';', errorEvent.Reasons));
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task HandleExceptionAsync(Exception exception)
    {
        _logger.LogError(exception, "Exception is {Message}", exception.Message);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task HandleCompleteAsync()
    {
        _logger.LogInformation($"Stream {Constants.PurchasesNamespace} is completed.");
        return Task.CompletedTask;
    }

    private async Task ApplyEventAsync(PurchaseInitializedEvent purchaseEvent)
    {
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = $"System/{GetType().Name}";
        try
        {
            var purchaseInfo = await _dbContext.Purchases.FindAsync(purchaseEvent.PurchaseId);
            if (purchaseInfo == null)
            {
                purchaseInfo = new PurchaseInfo
                               {
                                   Id = purchaseEvent.PurchaseId,
                                   MachineId = purchaseEvent.MachineId,
                                   Position = purchaseEvent.Position,
                                   SnackId = purchaseEvent.SnackId,
                                   BoughtPrice = purchaseEvent.BoughtPrice,
                                   BoughtAt = purchaseEvent.OperatedAt,
                                   BoughtBy = purchaseEvent.OperatedBy,
                                   Version = purchaseEvent.Version
                               };
                _dbContext.Purchases.Add(purchaseInfo);
            }
            if (_dbContext.Entry(purchaseInfo).State != EntityState.Added)
            {
                _logger.LogWarning("Apply PurchaseInitializedEvent: PurchaseInfo {PurchaseId} is already in the database. Try to execute full update...", purchaseEvent.PurchaseId);
                await ApplyFullUpdateAsync(purchaseEvent);
                return;
            }
            await purchaseInfo.UpdateSnackNameAndPictureUrlAsync(GetSnackNameAndPictureUrlAsync);
            await _dbContext.SaveChangesAsync();
            await PublishAsync(new PurchaseInfoSavedEvent(purchaseInfo.Id, purchaseInfo.Version, purchaseInfo, purchaseEvent.TraceId, operatedAt, operatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply PurchaseInitializedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new PurchaseInfoErrorEvent(purchaseEvent.PurchaseId, purchaseEvent.Version, 301, new[] { ex.Message }, purchaseEvent.TraceId, operatedAt, operatedBy));
            await ApplyFullUpdateAsync(purchaseEvent);
        }
    }

    private async Task ApplyFullUpdateAsync(PurchaseEvent purchaseEvent)
    {
        var attempts = 0;
        bool retryNeeded;
        do
        {
            var operatedAt = DateTimeOffset.UtcNow;
            var operatedBy = $"System/{GetType().Name}";
            try
            {
                var purchaseGrain = GrainFactory.GetGrain<IPurchaseGrain>(purchaseEvent.PurchaseId);
                var purchase = await purchaseGrain.GetPurchaseAsync();
                var purchaseInfo = await _dbContext.Purchases.FindAsync(purchaseEvent.PurchaseId);
                if (purchase == null || purchase.Id == Guid.Empty)
                {
                    if (purchaseInfo == null)
                    {
                        return;
                    }
                    _dbContext.Remove(purchaseInfo);
                    await _dbContext.SaveChangesAsync();
                    return;
                }
                if (purchaseInfo == null)
                {
                    purchaseInfo = new PurchaseInfo();
                    _dbContext.Purchases.Add(purchaseInfo);
                }
                await purchase.ToProjection(GetSnackNameAndPictureUrlAsync, purchaseInfo);
                await _dbContext.SaveChangesAsync();
                await PublishAsync(new PurchaseInfoSavedEvent(purchaseInfo.Id, purchaseInfo.Version, purchaseInfo, purchaseEvent.TraceId, operatedAt, operatedBy));
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
                await PublishErrorAsync(new PurchaseInfoErrorEvent(purchaseEvent.PurchaseId, purchaseEvent.Version, 300, new[] { ex.Message }, purchaseEvent.TraceId, operatedAt, operatedBy));
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