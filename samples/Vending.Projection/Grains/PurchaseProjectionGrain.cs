﻿using System.Collections.Concurrent;
using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Events;
using Vending.Domain.Abstractions.Grains;
using Vending.Projection.Abstractions.Entities;
using Vending.Projection.Abstractions.Mappers;
using Vending.Projection.EntityFrameworkCore;

namespace Vending.Projection.Grains;

[ImplicitStreamSubscription(Constants.PurchasesNamespace)]
public sealed class PurchaseProjectionGrain : SubscriberGrain<PurchaseEvent, PurchaseErrorEvent>
{
    private readonly ProjectionDbContext _dbContext;
    private readonly ILogger<PurchaseProjectionGrain> _logger;

    /// <inheritdoc />
    public PurchaseProjectionGrain(ProjectionDbContext dbContext, ILogger<PurchaseProjectionGrain> logger) : base(Constants.StreamProviderName2)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    protected override string GetSubscribeStreamNamespace()
    {
        return Constants.PurchasesNamespace;
    }

    /// <inheritdoc />
    protected override Task HandLeEventAsync(PurchaseEvent domainEvent)
    {
        switch (domainEvent)
        {
            case PurchaseInitializedEvent purchaseEvent:
                return ApplyEventAsync(purchaseEvent);
            default:
                return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    protected override Task HandLeErrorEventAsync(PurchaseErrorEvent errorEvent)
    {
        _logger.LogWarning($"PurchaseErrorEvent received: {string.Join(';', errorEvent.Reasons)}");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task HandleExceptionAsync(Exception exception)
    {
        _logger.LogError(exception, exception.Message);
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
        try
        {
            var purchase = await _dbContext.Purchases.FindAsync(purchaseEvent.MachineId, purchaseEvent.Position, purchaseEvent.SnackId);
            if (purchase == null)
            {
                purchase = new Purchase
                           {
                               MachineId = purchaseEvent.MachineId,
                               Position = purchaseEvent.Position,
                               SnackId = purchaseEvent.SnackId,
                               BoughtPrice = purchaseEvent.BoughtPrice,
                               BoughtAt = purchaseEvent.OperatedAt,
                               BoughtBy = purchaseEvent.OperatedBy
                           };
                _dbContext.Purchases.Add(purchase);
            }
            if (_dbContext.Entry(purchase).State != EntityState.Added)
            {
                _logger.LogWarning($"Apply PurchaseInitializedEvent: Purchase {purchaseEvent.Id} is already in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(purchaseEvent);
                return;
            }
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply PurchaseInitializedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(purchaseEvent);
        }
    }

    private async Task ApplyFullUpdateAsync(PurchaseEvent purchaseEvent)
    {
        var attempts = 0;
        bool retryNeeded;
        do
        {
            try
            {
                var id = purchaseEvent.PurchaseId;
                var purchaseGrain = GrainFactory.GetGrain<IPurchaseGrain>(id);
                var purchaseInGrain = await purchaseGrain.GetStateAsync();
                var purchase = await _dbContext.Purchases.FindAsync(purchaseEvent.MachineId, purchaseEvent.Position, purchaseEvent.SnackId);
                if (purchaseInGrain == null)
                {
                    if (purchase == null)
                    {
                        return;
                    }
                    _dbContext.Remove(purchase);
                    await _dbContext.SaveChangesAsync();
                    return;
                }
                if (purchase == null)
                {
                    purchase = new Purchase();
                    _dbContext.Purchases.Add(purchase);
                }
                await purchaseInGrain.ToProjection(GetSnackNameAndPictureUrlAsync, purchase);
                await _dbContext.SaveChangesAsync();
                return;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                retryNeeded = ++attempts <= 3;
                if (retryNeeded)
                {
                    _logger.LogWarning(ex, $"ApplyFullUpdateAsync: DbUpdateConcurrencyException is occurred when try to write data to the database. Retrying {attempts}...");
                    await Task.Delay(TimeSpan.FromSeconds(attempts));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ApplyFullUpdateAsync: Exception is occurred when try to write data to the database.");
                retryNeeded = false;
            }
        }
        while (retryNeeded);
    }

    #region Get Snack Name And Picture Url

    private readonly ConcurrentDictionary<Guid, (string SnackName, string? SnackPictureUrl)> _snackNamePictureCache = new();

    private async Task<(string SnackName, string? SnackPictureUrl)> GetSnackNameAndPictureUrlAsync(Guid snackId)
    {
        if (!_snackNamePictureCache.TryGetValue(snackId, out var snackNamePicture))
        {
            var snackGrain = GrainFactory.GetGrain<ISnackGrain>(snackId);
            var snackInGrain = await snackGrain.GetStateAsync();
            snackNamePicture = (snackInGrain.Name, snackInGrain.PictureUrl);
            _snackNamePictureCache.TryAdd(snackId, snackNamePicture);
        }
        return snackNamePicture;
    }

    #endregion

}