using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Snacks;
using Vending.Projection.Abstractions.Snacks;
using Vending.Projection.EntityFrameworkCore;

namespace Vending.Projection.Snacks;

[ImplicitStreamSubscription(Constants.SnacksNamespace)]
public sealed class SnackProjectionGrain : SubscriberPublisherGrainWithGuidKey<SnackEvent, SnackErrorEvent, SnackInfoEvent, SnackInfoErrorEvent>, ISnackProjectionGrain
{
    private readonly ProjectionDbContext _dbContext;
    private readonly ILogger<SnackProjectionGrain> _logger;

    /// <inheritdoc />
    public SnackProjectionGrain(ProjectionDbContext dbContext, ILogger<SnackProjectionGrain> logger)
        : base(Constants.StreamProviderName)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    protected override string GetSubStreamNamespace()
    {
        return Constants.SnacksNamespace;
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
    protected override Task HandLeEventAsync(SnackEvent domainEvent)
    {
        switch (domainEvent)
        {
            case SnackInitializedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case SnackDeletedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case SnackUpdatedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case SnackMachineCountUpdatedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case SnackTotalQuantityUpdatedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case SnackTotalAmountUpdatedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case SnackBoughtCountUpdatedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case SnackBoughtAmountUpdatedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            default:
                return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    protected override Task HandLeErrorEventAsync(SnackErrorEvent errorEvent)
    {
        _logger.LogWarning("SnackErrorEvent received: {Reasons}", string.Join(';', errorEvent.Reasons));
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
        _logger.LogInformation($"Stream {Constants.SnacksNamespace} is completed.");
        return Task.CompletedTask;
    }

    private async Task ApplyEventAsync(SnackInitializedEvent snackEvent)
    {
        try
        {
            var snackInfo = await _dbContext.Snacks.FindAsync(snackEvent.SnackId);
            if (snackInfo == null)
            {
                snackInfo = new SnackInfo
                            {
                                Id = snackEvent.SnackId,
                                Name = snackEvent.Name,
                                PictureUrl = snackEvent.PictureUrl,
                                CreatedAt = snackEvent.OperatedAt,
                                CreatedBy = snackEvent.OperatedBy,
                                Version = snackEvent.Version
                            };
                _dbContext.Snacks.Add(snackInfo);
            }
            if (_dbContext.Entry(snackInfo).State != EntityState.Added)
            {
                _logger.LogWarning("Apply SnackInitializedEvent: Snack {SnackId} is already in the database. Try to execute full update...", snackEvent.SnackId);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            await _dbContext.SaveChangesAsync();
            await PublishAsync(new SnackInfoSavedEvent(snackInfo.Id, snackInfo.Version, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackInitializedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new SnackInfoErrorEvent(snackEvent.SnackId, snackEvent.Version, 101, new[] { ex.Message }, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
            await ApplyFullUpdateAsync(snackEvent);
        }
    }

    private async Task ApplyEventAsync(SnackDeletedEvent snackEvent)
    {
        try
        {
            var snackInfo = await _dbContext.Snacks.FindAsync(snackEvent.SnackId);
            if (snackInfo == null)
            {
                _logger.LogWarning("Apply SnackDeletedEvent: Snack {SnackId} does not exist in the database. Try to execute full update...", snackEvent.SnackId);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            if (snackInfo.Version != snackEvent.Version - 1)
            {
                _logger.LogWarning("Apply SnackDeletedEvent: Snack {SnackId} version {Version}) in the database should be {SnackVersion}. Try to execute full update...", snackEvent.SnackId, snackInfo.Version, snackEvent.Version - 1);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            snackInfo.DeletedAt = snackEvent.OperatedAt;
            snackInfo.DeletedBy = snackEvent.OperatedBy;
            snackInfo.IsDeleted = true;
            snackInfo.Version = snackEvent.Version;
            await _dbContext.SaveChangesAsync();
            await PublishAsync(new SnackInfoSavedEvent(snackInfo.Id, snackInfo.Version, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackDeletedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new SnackInfoErrorEvent(snackEvent.SnackId, snackEvent.Version, 102, new[] { ex.Message }, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
            await ApplyFullUpdateAsync(snackEvent);
        }
    }

    private async Task ApplyEventAsync(SnackUpdatedEvent snackEvent)
    {
        try
        {
            var snackInfo = await _dbContext.Snacks.FindAsync(snackEvent.SnackId);
            if (snackInfo == null)
            {
                _logger.LogWarning("Apply SnackUpdatedEvent: Snack {SnackId} does not exist in the database. Try to execute full update...", snackEvent.SnackId);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            if (snackInfo.Version != snackEvent.Version - 1)
            {
                _logger.LogWarning("Apply SnackUpdatedEvent: Snack {SnackId} version {Version}) in the database should be {SnackVersion}. Try to execute full update...", snackEvent.SnackId, snackInfo.Version, snackEvent.Version - 1);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            snackInfo.Name = snackEvent.Name;
            snackInfo.PictureUrl = snackEvent.PictureUrl;
            snackInfo.LastModifiedAt = snackEvent.OperatedAt;
            snackInfo.LastModifiedBy = snackEvent.OperatedBy;
            snackInfo.Version = snackEvent.Version;
            await _dbContext.SaveChangesAsync();
            await PublishAsync(new SnackInfoSavedEvent(snackInfo.Id, snackInfo.Version, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new SnackInfoErrorEvent(snackEvent.SnackId, snackEvent.Version, 103, new[] { ex.Message }, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
            await ApplyFullUpdateAsync(snackEvent);
        }
    }

    private async Task ApplyEventAsync(SnackMachineCountUpdatedEvent snackEvent)
    {
        try
        {
            var snackInfo = await _dbContext.Snacks.FindAsync(snackEvent.SnackId);
            if (snackInfo == null)
            {
                _logger.LogWarning("Apply SnackMachineCountUpdatedEvent: Snack {SnackId} does not exist in the database. Try to execute full update...", snackEvent.SnackId);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            snackInfo.MachineCount = snackEvent.MachineCount;
            await _dbContext.SaveChangesAsync();
            await PublishAsync(new SnackInfoSavedEvent(snackInfo.Id, snackInfo.Version, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackMachineCountUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new SnackInfoErrorEvent(snackEvent.SnackId, snackEvent.Version, 121, new[] { ex.Message }, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
            await ApplyFullUpdateAsync(snackEvent);
        }
    }

    private async Task ApplyEventAsync(SnackTotalQuantityUpdatedEvent snackEvent)
    {
        try
        {
            var snackInfo = await _dbContext.Snacks.FindAsync(snackEvent.SnackId);
            if (snackInfo == null)
            {
                _logger.LogWarning("Apply SnackTotalQuantityUpdatedEvent: Snack {SnackId} does not exist in the database. Try to execute full update...", snackEvent.SnackId);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            snackInfo.TotalQuantity = snackEvent.TotalQuantity;
            await _dbContext.SaveChangesAsync();
            await PublishAsync(new SnackInfoSavedEvent(snackInfo.Id, snackInfo.Version, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackTotalQuantityUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new SnackInfoErrorEvent(snackEvent.SnackId, snackEvent.Version, 122, new[] { ex.Message }, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
            await ApplyFullUpdateAsync(snackEvent);
        }
    }

    private async Task ApplyEventAsync(SnackTotalAmountUpdatedEvent snackEvent)
    {
        try
        {
            var snackInfo = await _dbContext.Snacks.FindAsync(snackEvent.SnackId);
            if (snackInfo == null)
            {
                _logger.LogWarning("Apply SnackTotalAmountUpdatedEvent: Snack {SnackId} does not exist in the database. Try to execute full update...", snackEvent.SnackId);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            snackInfo.TotalAmount = snackEvent.TotalAmount;
            await _dbContext.SaveChangesAsync();
            await PublishAsync(new SnackInfoSavedEvent(snackInfo.Id, snackInfo.Version, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackTotalAmountUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new SnackInfoErrorEvent(snackEvent.SnackId, snackEvent.Version, 123, new[] { ex.Message }, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
            await ApplyFullUpdateAsync(snackEvent);
        }
    }

    private async Task ApplyEventAsync(SnackBoughtCountUpdatedEvent snackEvent)
    {
        try
        {
            var snackInfo = await _dbContext.Snacks.FindAsync(snackEvent.SnackId);
            if (snackInfo == null)
            {
                _logger.LogWarning("Apply SnackBoughtCountUpdatedEvent: Snack {SnackId} does not exist in the database. Try to execute full update...", snackEvent.SnackId);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            snackInfo.BoughtCount = snackEvent.BoughtCount;
            await _dbContext.SaveChangesAsync();
            await PublishAsync(new SnackInfoSavedEvent(snackInfo.Id, snackInfo.Version, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackBoughtCountUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new SnackInfoErrorEvent(snackEvent.SnackId, snackEvent.Version, 131, new[] { ex.Message }, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
            await ApplyFullUpdateAsync(snackEvent);
        }
    }

    private async Task ApplyEventAsync(SnackBoughtAmountUpdatedEvent snackEvent)
    {
        try
        {
            var snackInfo = await _dbContext.Snacks.FindAsync(snackEvent.SnackId);
            if (snackInfo == null)
            {
                _logger.LogWarning("Apply SnackBoughtAmountUpdatedEvent: Snack {SnackId} does not exist in the database. Try to execute full update...", snackEvent.SnackId);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            snackInfo.BoughtAmount = snackEvent.BoughtAmount;
            await _dbContext.SaveChangesAsync();
            await PublishAsync(new SnackInfoSavedEvent(snackInfo.Id, snackInfo.Version, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackBoughtAmountUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new SnackInfoErrorEvent(snackEvent.SnackId, snackEvent.Version, 132, new[] { ex.Message }, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
            await ApplyFullUpdateAsync(snackEvent);
        }
    }

    private async Task ApplyFullUpdateAsync(SnackEvent snackEvent)
    {
        var attempts = 0;
        bool retryNeeded;
        do
        {
            try
            {
                var snackId = snackEvent.SnackId;
                var snackGrain = GrainFactory.GetGrain<ISnackGrain>(snackId);
                var snack = await snackGrain.GetSnackAsync();
                var snackInfo = await _dbContext.Snacks.FindAsync(snackId);
                if (snack == null)
                {
                    if (snackInfo == null)
                    {
                        return;
                    }
                    _dbContext.Remove(snackInfo);
                    await _dbContext.SaveChangesAsync();
                    return;
                }
                if (snackInfo == null)
                {
                    snackInfo = new SnackInfo();
                    _dbContext.Snacks.Add(snackInfo);
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
                await _dbContext.SaveChangesAsync();
                await PublishAsync(new SnackInfoSavedEvent(snackInfo.Id, snackInfo.Version, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
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
                await PublishErrorAsync(new SnackInfoErrorEvent(snackEvent.SnackId, snackEvent.Version, 100, new[] { ex.Message }, snackEvent.TraceId, DateTimeOffset.UtcNow, snackEvent.OperatedBy));
                retryNeeded = false;
            }
        }
        while (retryNeeded);
    }
}
