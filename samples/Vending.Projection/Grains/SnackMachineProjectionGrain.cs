using System.Collections.Concurrent;
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

[ImplicitStreamSubscription(Constants.SnackMachinesNamespace)]
public sealed class SnackMachineProjectionGrain : EventSubscriberGrain<SnackMachineEvent, SnackMachineErrorEvent>
{
    private readonly ProjectionDbContext _dbContext;
    private readonly ILogger<SnackMachineProjectionGrain> _logger;

    /// <inheritdoc />
    public SnackMachineProjectionGrain(ProjectionDbContext dbContext, ILogger<SnackMachineProjectionGrain> logger) : base(Constants.StreamProviderName2)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    protected override string GetStreamNamespace()
    {
        return Constants.SnackMachinesNamespace;
    }

    /// <inheritdoc />
    protected override Task HandLeEventAsync(SnackMachineEvent domainEvent)
    {
        switch (domainEvent)
        {
            case SnackMachineInitializedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case SnackMachineRemovedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case SnackMachineMoneyLoadedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case SnackMachineMoneyUnloadedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case SnackMachineMoneyInsertedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case SnackMachineMoneyReturnedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case SnackMachineSnacksLoadedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case SnackMachineSnackBoughtEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            default:
                return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    protected override Task HandLeErrorEventAsync(SnackMachineErrorEvent errorEvent)
    {
        _logger.LogWarning($"SnackMachineErrorEvent received: {string.Join(';', errorEvent.Reasons)}");
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

    private async Task ApplyEventAsync(SnackMachineInitializedEvent machineEvent)
    {
        try
        {
            var snackMachine = await _dbContext.SnackMachines.FindAsync(machineEvent.Id);
            if (snackMachine == null)
            {
                snackMachine = new SnackMachine
                               {
                                   Id = machineEvent.Id,
                                   MoneyInside = machineEvent.MoneyInside.ToProjection(),
                                   Slots = await Task.WhenAll(machineEvent.Slots.Select(slot => slot.ToProjection(GetSnackNameAndPictureUrlAsync))),
                                   SlotsCount = machineEvent.SlotsCount,
                                   SnackCount = machineEvent.SnackCount,
                                   SnackQuantity = machineEvent.SnackQuantity,
                                   SnackAmount = machineEvent.SnackAmount,
                                   CreatedAt = machineEvent.OperatedAt,
                                   CreatedBy = machineEvent.OperatedBy,
                                   Version = machineEvent.Version
                               };
                _dbContext.SnackMachines.Add(snackMachine);
            }
            if (_dbContext.Entry(snackMachine).State != EntityState.Added)
            {
                _logger.LogWarning($"Apply SnackMachineInitializedEvent: Snack machine {machineEvent.Id} is already in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackMachineInitializedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(SnackMachineRemovedEvent machineEvent)
    {
        try
        {
            var snackMachine = await _dbContext.SnackMachines.FindAsync(machineEvent.Id);
            if (snackMachine == null)
            {
                _logger.LogWarning($"Apply SnackMachineRemovedEvent: Snack machine {machineEvent.Id} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (snackMachine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackMachineRemovedEvent: Snack machine {machineEvent.Id} version {snackMachine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            snackMachine.DeletedAt = machineEvent.OperatedAt;
            snackMachine.DeletedBy = machineEvent.OperatedBy;
            snackMachine.IsDeleted = true;
            snackMachine.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackMachineRemovedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(SnackMachineMoneyLoadedEvent machineEvent)
    {
        try
        {
            var snackMachine = await _dbContext.SnackMachines.FindAsync(machineEvent.Id);
            if (snackMachine == null)
            {
                _logger.LogWarning($"Apply SnackMachineMoneyLoadedEvent: Snack machine {machineEvent.Id} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (snackMachine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackMachineMoneyLoadedEvent: Snack machine {machineEvent.Id} version {snackMachine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            snackMachine.MoneyInside = machineEvent.MoneyInside.ToProjection(snackMachine.MoneyInside);
            snackMachine.LastModifiedAt = machineEvent.OperatedAt;
            snackMachine.LastModifiedBy = machineEvent.OperatedBy;
            snackMachine.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackMachineMoneyLoadedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(SnackMachineMoneyUnloadedEvent machineEvent)
    {
        try
        {
            var snackMachine = await _dbContext.SnackMachines.FindAsync(machineEvent.Id);
            if (snackMachine == null)
            {
                _logger.LogWarning($"Apply SnackMachineMoneyUnloadedEvent: Snack machine {machineEvent.Id} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (snackMachine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackMachineMoneyUnloadedEvent: Snack machine {machineEvent.Id} version {snackMachine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            snackMachine.MoneyInside = machineEvent.MoneyInside.ToProjection(snackMachine.MoneyInside);
            snackMachine.LastModifiedAt = machineEvent.OperatedAt;
            snackMachine.LastModifiedBy = machineEvent.OperatedBy;
            snackMachine.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackMachineMoneyUnloadedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(SnackMachineMoneyInsertedEvent machineEvent)
    {
        try
        {
            var snackMachine = await _dbContext.SnackMachines.FindAsync(machineEvent.Id);
            if (snackMachine == null)
            {
                _logger.LogWarning($"Apply SnackMachineMoneyInsertedEvent: Snack machine {machineEvent.Id} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (snackMachine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackMachineMoneyInsertedEvent: Snack machine {machineEvent.Id} version {snackMachine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            snackMachine.MoneyInside = machineEvent.MoneyInside.ToProjection(snackMachine.MoneyInside);
            snackMachine.AmountInTransaction = machineEvent.AmountInTransaction;
            snackMachine.LastModifiedAt = machineEvent.OperatedAt;
            snackMachine.LastModifiedBy = machineEvent.OperatedBy;
            snackMachine.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackMachineMoneyInsertedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(SnackMachineMoneyReturnedEvent machineEvent)
    {
        try
        {
            var snackMachine = await _dbContext.SnackMachines.FindAsync(machineEvent.Id);
            if (snackMachine == null)
            {
                _logger.LogWarning($"Apply SnackMachineMoneyReturnedEvent: Snack machine {machineEvent.Id} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (snackMachine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackMachineMoneyReturnedEvent: Snack machine {machineEvent.Id} version {snackMachine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            snackMachine.MoneyInside = machineEvent.MoneyInside.ToProjection(snackMachine.MoneyInside);
            snackMachine.AmountInTransaction = machineEvent.AmountInTransaction;
            snackMachine.LastModifiedAt = machineEvent.OperatedAt;
            snackMachine.LastModifiedBy = machineEvent.OperatedBy;
            snackMachine.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackMachineMoneyReturnedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(SnackMachineSnacksLoadedEvent machineEvent)
    {
        try
        {
            var snackMachine = await _dbContext.SnackMachines.Include(sm => sm.Slots).FirstOrDefaultAsync(sm => sm.Id == machineEvent.Id);
            if (snackMachine == null)
            {
                _logger.LogWarning($"Apply SnackMachineSnacksLoadedEvent: Snack machine {machineEvent.Id} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (snackMachine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackMachineSnacksLoadedEvent: Snack machine {machineEvent.Id} version {snackMachine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            var slot = snackMachine.Slots.FirstOrDefault(sl => sl.MachineId == machineEvent.Slot.MachineId && sl.Position == machineEvent.Slot.Position);
            if (slot == null)
            {
                slot = new Slot();
                snackMachine.Slots.Add(slot);
            }
            await machineEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync, slot);
            snackMachine.SlotsCount = machineEvent.SlotsCount;
            snackMachine.SnackCount = machineEvent.SnackCount;
            snackMachine.SnackQuantity = machineEvent.SnackQuantity;
            snackMachine.SnackAmount = machineEvent.SnackAmount;
            snackMachine.LastModifiedAt = machineEvent.OperatedAt;
            snackMachine.LastModifiedBy = machineEvent.OperatedBy;
            snackMachine.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackMachineMoneyReturnedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(SnackMachineSnackBoughtEvent machineEvent)
    {
        try
        {
            var snackMachine = await _dbContext.SnackMachines.Include(sm => sm.Slots).FirstOrDefaultAsync(sm => sm.Id == machineEvent.Id);
            if (snackMachine == null)
            {
                _logger.LogWarning($"Apply SnackMachineSnackBoughtEvent: Snack machine {machineEvent.Id} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (snackMachine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackMachineSnackBoughtEvent: Snack machine {machineEvent.Id} version {snackMachine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            snackMachine.AmountInTransaction = machineEvent.AmountInTransaction;
            var slot = snackMachine.Slots.FirstOrDefault(sl => sl.MachineId == machineEvent.Slot.MachineId && sl.Position == machineEvent.Slot.Position);
            if (slot == null)
            {
                slot = new Slot();
                snackMachine.Slots.Add(slot);
            }
            await machineEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync, slot);
            snackMachine.SnackQuantity = machineEvent.SnackQuantity;
            snackMachine.SnackAmount = machineEvent.SnackAmount;
            snackMachine.LastModifiedAt = machineEvent.OperatedAt;
            snackMachine.LastModifiedBy = machineEvent.OperatedBy;
            snackMachine.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackMachineMoneyReturnedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyFullUpdateAsync(SnackMachineEvent machineEvent)
    {
        var attempts = 0;
        bool retryNeeded;
        do
        {
            try
            {
                var id = machineEvent.Id;
                var snackMachineGrain = GrainFactory.GetGrain<ISnackMachineGrain>(id);
                var snackMachineInGrain = await snackMachineGrain.GetStateAsync();
                var snackMachine = await _dbContext.SnackMachines.FindAsync(id);
                if (snackMachineInGrain == null)
                {
                    if (snackMachine == null)
                    {
                        return;
                    }
                    _dbContext.Remove(snackMachine);
                    await _dbContext.SaveChangesAsync();
                    return;
                }
                if (snackMachine == null)
                {
                    snackMachine = new SnackMachine();
                    _dbContext.SnackMachines.Add(snackMachine);
                }
                snackMachine = await snackMachineInGrain.ToProjection(GetSnackNameAndPictureUrlAsync, snackMachine);
                snackMachine.Version = await snackMachineGrain.GetVersionAsync();
                // TODO: Get the following data from the grain
                // snackMachine.BoughtCount = await _dbContext.SnacksBoughts.CountAsync(sb => sb.MachineId == id);
                // snackMachine.BoughtAmount = await _dbContext.SnacksBoughts.Where(sb => sb.MachineId == id).SumAsync(sb => sb.BoughtPrice);
                await _dbContext.SaveChangesAsync();
                return;
            }
            catch (DbUpdateConcurrencyException)
            {
                retryNeeded = ++attempts <= 3;
                if (retryNeeded)
                {
                    _logger.LogWarning($"Apply FullUpdate: DbUpdateConcurrencyException is occurred when try to write data to the database. Retrying {attempts}...");
                    await Task.Delay(TimeSpan.FromSeconds(attempts));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Apply FullUpdate: Exception is occurred when try to write data to the database.");
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
