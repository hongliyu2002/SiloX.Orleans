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
public sealed class SnackProjectionGrain : SubscriberGrainWithGuidKey<SnackEvent, SnackErrorEvent>, ISnackProjectionGrain
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
    protected override string GetStreamNamespace()
    {
        return Constants.SnacksNamespace;
    }

    /// <inheritdoc />
    protected override Task HandLeEventAsync(SnackEvent domainEvent)
    {
        switch (domainEvent)
        {
            case SnackInitializedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case SnackRemovedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case SnackUpdatedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case MachineCountUpdatedEvent snackEvent:
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
            var snack = await _dbContext.Snacks.FindAsync(snackEvent.SnackId);
            if (snack == null)
            {
                snack = new SnackInfo
                        {
                            Id = snackEvent.SnackId,
                            Name = snackEvent.Name,
                            PictureUrl = snackEvent.PictureUrl,
                            CreatedAt = snackEvent.OperatedAt,
                            CreatedBy = snackEvent.OperatedBy,
                            Version = snackEvent.Version
                        };
                _dbContext.Snacks.Add(snack);
            }
            if (_dbContext.Entry(snack).State != EntityState.Added)
            {
                _logger.LogWarning("Apply SnackInitializedEvent: SnackInfo {SnackId} is already in the database. Try to execute full update...", snackEvent.SnackId);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackInitializedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(snackEvent);
        }
    }

    private async Task ApplyEventAsync(SnackRemovedEvent snackEvent)
    {
        try
        {
            var snack = await _dbContext.Snacks.FindAsync(snackEvent.SnackId);
            if (snack == null)
            {
                _logger.LogWarning("Apply SnackRemovedEvent: SnackInfo {SnackId} does not exist in the database. Try to execute full update...", snackEvent.SnackId);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            if (snack.Version != snackEvent.Version - 1)
            {
                _logger.LogWarning("Apply SnackRemovedEvent: SnackInfo {SnackId} version {Version}) in the database should be {SnackVersion}. Try to execute full update...", snackEvent.SnackId, snack.Version, snackEvent.Version - 1);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            snack.DeletedAt = snackEvent.OperatedAt;
            snack.DeletedBy = snackEvent.OperatedBy;
            snack.IsDeleted = true;
            snack.Version = snackEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackRemovedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(snackEvent);
        }
    }

    private async Task ApplyEventAsync(SnackUpdatedEvent snackEvent)
    {
        try
        {
            var snack = await _dbContext.Snacks.FindAsync(snackEvent.SnackId);
            if (snack == null)
            {
                _logger.LogWarning("Apply SnackUpdatedEvent: SnackInfo {SnackId} does not exist in the database. Try to execute full update...", snackEvent.SnackId);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            if (snack.Version != snackEvent.Version - 1)
            {
                _logger.LogWarning("Apply SnackUpdatedEvent: SnackInfo {SnackId} version {Version}) in the database should be {SnackVersion}. Try to execute full update...", snackEvent.SnackId, snack.Version, snackEvent.Version - 1);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            snack.Name = snackEvent.Name;
            snack.PictureUrl = snackEvent.PictureUrl;
            snack.LastModifiedAt = snackEvent.OperatedAt;
            snack.LastModifiedBy = snackEvent.OperatedBy;
            snack.Version = snackEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(snackEvent);
        }
    }

    private async Task ApplyEventAsync(MachineCountUpdatedEvent snackEvent)
    {
        try
        {
            var snack = await _dbContext.Snacks.FindAsync(snackEvent.SnackId);
            if (snack == null)
            {
                _logger.LogWarning("Apply MachineCountUpdatedEvent: SnackInfo {SnackId} does not exist in the database. Try to execute full update...", snackEvent.SnackId);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            snack.MachineCount = snackEvent.Count;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineCountUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(snackEvent);
        }
    }

    private async Task ApplyEventAsync(SnackBoughtCountUpdatedEvent snackEvent)
    {
        try
        {
            var snack = await _dbContext.Snacks.FindAsync(snackEvent.SnackId);
            if (snack == null)
            {
                _logger.LogWarning("Apply SnackBoughtCountUpdatedEvent: SnackInfo {SnackId} does not exist in the database. Try to execute full update...", snackEvent.SnackId);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            snack.BoughtCount = snackEvent.Count;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackBoughtCountUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(snackEvent);
        }
    }

    private async Task ApplyEventAsync(SnackBoughtAmountUpdatedEvent snackEvent)
    {
        try
        {
            var snack = await _dbContext.Snacks.FindAsync(snackEvent.SnackId);
            if (snack == null)
            {
                _logger.LogWarning("Apply SnackBoughtAmountUpdatedEvent: SnackInfo {SnackId} does not exist in the database. Try to execute full update...", snackEvent.SnackId);
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            snack.BoughtAmount = snackEvent.Amount;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackBoughtAmountUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
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
                var snackInGrain = await snackGrain.GetStateAsync();
                var snack = await _dbContext.Snacks.FindAsync(snackId);
                if (snackInGrain == null)
                {
                    if (snack == null)
                    {
                        return;
                    }
                    _dbContext.Remove(snack);
                    await _dbContext.SaveChangesAsync();
                    return;
                }
                if (snack == null)
                {
                    snack = new SnackInfo();
                    _dbContext.Snacks.Add(snack);
                }
                snack = snackInGrain.ToProjection(snack);
                snack.Version = await snackGrain.GetVersionAsync();
                var machineStatsGrain = GrainFactory.GetGrain<ISnackStatsOfMachineGrain>(snackId);
                snack.MachineCount = await machineStatsGrain.GetCountAsync();
                var purchaseStatsGrain = GrainFactory.GetGrain<ISnackStatsOfPurchasesGrain>(snackId);
                snack.BoughtCount = await purchaseStatsGrain.GetCountAsync();
                snack.BoughtAmount = await purchaseStatsGrain.GetAmountAsync();
                await _dbContext.SaveChangesAsync();
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
                retryNeeded = false;
            }
        }
        while (retryNeeded);
    }
}
