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

[ImplicitStreamSubscription(Constants.SnacksNamespace)]
public sealed class SnackProjectionGrain : SubscriberGrain<SnackEvent, SnackErrorEvent>
{
    private readonly ProjectionDbContext _dbContext;
    private readonly ILogger<SnackProjectionGrain> _logger;

    /// <inheritdoc />
    public SnackProjectionGrain(ProjectionDbContext dbContext, ILogger<SnackProjectionGrain> logger) : base(Constants.StreamProviderName1)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    protected override string GetSubscribeStreamNamespace()
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
            case SnackNameChangedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case SnackPictureUrlChangedEvent snackEvt:
                return ApplyEventAsync(snackEvt);
            case SnackStatsMachineCountUpdatedEvent snackEvt:
                return ApplyEventAsync(snackEvt);
            case SnackStatsBoughtCountUpdatedEvent snackEvt:
                return ApplyEventAsync(snackEvt);
            case SnackStatsBoughtAmountUpdatedEvent snackEvt:
                return ApplyEventAsync(snackEvt);
            default:
                return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    protected override Task HandLeErrorEventAsync(SnackErrorEvent errorEvent)
    {
        _logger.LogWarning($"SnackErrorEvent received: {string.Join(';', errorEvent.Reasons)}");
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
        _logger.LogInformation($"Stream {Constants.SnacksNamespace} is completed.");
        return Task.CompletedTask;
    }

    private async Task ApplyEventAsync(SnackInitializedEvent snackEvent)
    {
        try
        {
            var snack = await _dbContext.Snacks.FindAsync(snackEvent.Id);
            if (snack == null)
            {
                snack = new Snack
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
                _logger.LogWarning($"Apply SnackInitializedEvent: Snack {snackEvent.Id} is already in the database. Try to execute full update...");
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
            var snack = await _dbContext.Snacks.FindAsync(snackEvent.Id);
            if (snack == null)
            {
                _logger.LogWarning($"Apply SnackRemovedEvent: Snack {snackEvent.Id} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            if (snack.Version != snackEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackRemovedEvent: Snack {snackEvent.Id} version {snack.Version}) in the database should be {snackEvent.Version - 1}. Try to execute full update...");
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

    private async Task ApplyEventAsync(SnackNameChangedEvent snackEvent)
    {
        try
        {
            var snack = await _dbContext.Snacks.FindAsync(snackEvent.Id);
            if (snack == null)
            {
                _logger.LogWarning($"Apply SnackNameChangedEvent: Snack {snackEvent.Id} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            if (snack.Version != snackEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackNameChangedEvent: Snack {snackEvent.Id} version {snack.Version}) in the database should be {snackEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            snack.Name = snackEvent.Name;
            snack.LastModifiedAt = snackEvent.OperatedAt;
            snack.LastModifiedBy = snackEvent.OperatedBy;
            snack.Version = snackEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackNameChangedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(snackEvent);
        }
    }

    private async Task ApplyEventAsync(SnackPictureUrlChangedEvent snackEvent)
    {
        try
        {
            var snack = await _dbContext.Snacks.FindAsync(snackEvent.Id);
            if (snack == null)
            {
                _logger.LogWarning($"Apply SnackPictureUrlChangedEvent: Snack {snackEvent.Id} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            if (snack.Version != snackEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackPictureUrlChangedEvent: Snack {snackEvent.Id} version {snack.Version}) in the database should be {snackEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            snack.PictureUrl = snackEvent.PictureUrl;
            snack.LastModifiedAt = snackEvent.OperatedAt;
            snack.LastModifiedBy = snackEvent.OperatedBy;
            snack.Version = snackEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackPictureUrlChangedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(snackEvent);
        }
    }

    private async Task ApplyEventAsync(SnackStatsMachineCountUpdatedEvent snackEvent)
    {
        try
        {
            var snack = await _dbContext.Snacks.FindAsync(snackEvent.Id);
            if (snack == null)
            {
                _logger.LogWarning($"Apply SnackStatsMachineCountUpdatedEvent: Snack {snackEvent.Id} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            snack.MachineCount = snackEvent.Count;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackStatsMachineCountUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(snackEvent);
        }
    }

    private async Task ApplyEventAsync(SnackStatsBoughtCountUpdatedEvent snackEvent)
    {
        try
        {
            var snack = await _dbContext.Snacks.FindAsync(snackEvent.Id);
            if (snack == null)
            {
                _logger.LogWarning($"Apply SnackStatsBoughtCountUpdatedEvent: Snack {snackEvent.Id} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            snack.BoughtCount = snackEvent.Count;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackStatsBoughtCountUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(snackEvent);
        }
    }

    private async Task ApplyEventAsync(SnackStatsBoughtAmountUpdatedEvent snackEvent)
    {
        try
        {
            var snack = await _dbContext.Snacks.FindAsync(snackEvent.Id);
            if (snack == null)
            {
                _logger.LogWarning($"Apply SnackStatsBoughtAmountUpdatedEvent: Snack {snackEvent.Id} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(snackEvent);
                return;
            }
            snack.BoughtAmount = snackEvent.Amount;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackStatsBoughtAmountUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
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
                var id = snackEvent.SnackId;
                var snackGrain = GrainFactory.GetGrain<ISnackGrain>(id);
                var snackInGrain = await snackGrain.GetStateAsync();
                var snack = await _dbContext.Snacks.FindAsync(id);
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
                    snack = new Snack();
                    _dbContext.Snacks.Add(snack);
                }
                snack = snackInGrain.ToProjection(snack);
                snack.Version = await snackGrain.GetVersionAsync();
                var snackMachineStatsBySnackGrain = GrainFactory.GetGrain<ISnackMachineStatsBySnackGrain>(id);
                snack.MachineCount = await snackMachineStatsBySnackGrain.GetMachineCountAsync();
                // TODO
                // snack.BoughtCount = await _dbContext.Purchases.CountAsync(sb => sb.SnackId == id);
                // snack.BoughtAmount = await _dbContext.Purchases.Where(sb => sb.SnackId == id).SumAsync(sb => sb.BoughtPrice);
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
}
