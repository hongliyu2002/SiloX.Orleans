using System.Collections.Concurrent;
using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.SnackMachines;
using Vending.Domain.Abstractions.Snacks;
using Vending.Projection.Abstractions.SnackMachines;
using Vending.Projection.EntityFrameworkCore;
using Slot = Vending.Projection.Abstractions.SnackMachines.Slot;
using SnackMachine = Vending.Projection.Abstractions.SnackMachines.SnackMachine;

namespace Vending.Projection.SnackMachines;

[ImplicitStreamSubscription(Constants.SnackMachinesNamespace)]
public sealed class SnackMachineProjectionGrain : SubscriberGrainWithGuidKey<SnackMachineEvent, SnackMachineErrorEvent>, ISnackMachineProjectionGrain
{
    private readonly ProjectionDbContext _dbContext;
    private readonly ILogger<SnackMachineProjectionGrain> _logger;

    /// <inheritdoc />
    public SnackMachineProjectionGrain(ProjectionDbContext dbContext, ILogger<SnackMachineProjectionGrain> logger)
        : base(Constants.StreamProviderName)
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
            case SnackMachineSnacksUnloadedEvent machineEvent:
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
        _logger.LogInformation($"Stream {Constants.SnackMachinesNamespace} is completed.");
        return Task.CompletedTask;
    }

    private async Task ApplyEventAsync(SnackMachineInitializedEvent machineEvent)
    {
        try
        {
            var machine = await _dbContext.SnackMachines.FindAsync(machineEvent.MachineId);
            if (machine == null)
            {
                machine = new SnackMachine
                          {
                              Id = machineEvent.MachineId,
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
                _dbContext.SnackMachines.Add(machine);
            }
            if (_dbContext.Entry(machine).State != EntityState.Added)
            {
                _logger.LogWarning($"Apply SnackMachineInitializedEvent: Snack machine {machineEvent.MachineId} is already in the database. Try to execute full update...");
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
            var machine = await _dbContext.SnackMachines.FindAsync(machineEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply SnackMachineRemovedEvent: Snack machine {machineEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackMachineRemovedEvent: Snack machine {machineEvent.MachineId} version {machine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machine.MoneyInside = machineEvent.MoneyInside.ToProjection(machine.MoneyInside);
            machine.AmountInTransaction = machineEvent.AmountInTransaction;
            machine.Slots = await Task.WhenAll(machineEvent.Slots.Select(slot => slot.ToProjection(GetSnackNameAndPictureUrlAsync, machine.Slots.FirstOrDefault(sl => sl.MachineId == slot.MachineId && sl.Position == slot.Position))));
            machine.SlotsCount = machineEvent.SlotsCount;
            machine.SnackCount = machineEvent.SnackCount;
            machine.SnackQuantity = machineEvent.SnackQuantity;
            machine.SnackAmount = machineEvent.SnackAmount;
            machine.DeletedAt = machineEvent.OperatedAt;
            machine.DeletedBy = machineEvent.OperatedBy;
            machine.IsDeleted = true;
            machine.Version = machineEvent.Version;
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
            var machine = await _dbContext.SnackMachines.FindAsync(machineEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply SnackMachineMoneyLoadedEvent: Snack machine {machineEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackMachineMoneyLoadedEvent: Snack machine {machineEvent.MachineId} version {machine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machine.MoneyInside = machineEvent.MoneyInside.ToProjection(machine.MoneyInside);
            machine.LastModifiedAt = machineEvent.OperatedAt;
            machine.LastModifiedBy = machineEvent.OperatedBy;
            machine.Version = machineEvent.Version;
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
            var machine = await _dbContext.SnackMachines.FindAsync(machineEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply SnackMachineMoneyUnloadedEvent: Snack machine {machineEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackMachineMoneyUnloadedEvent: Snack machine {machineEvent.MachineId} version {machine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machine.MoneyInside = machineEvent.MoneyInside.ToProjection(machine.MoneyInside);
            machine.LastModifiedAt = machineEvent.OperatedAt;
            machine.LastModifiedBy = machineEvent.OperatedBy;
            machine.Version = machineEvent.Version;
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
            var machine = await _dbContext.SnackMachines.FindAsync(machineEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply SnackMachineMoneyInsertedEvent: Snack machine {machineEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackMachineMoneyInsertedEvent: Snack machine {machineEvent.MachineId} version {machine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machine.MoneyInside = machineEvent.MoneyInside.ToProjection(machine.MoneyInside);
            machine.AmountInTransaction = machineEvent.AmountInTransaction;
            machine.LastModifiedAt = machineEvent.OperatedAt;
            machine.LastModifiedBy = machineEvent.OperatedBy;
            machine.Version = machineEvent.Version;
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
            var machine = await _dbContext.SnackMachines.FindAsync(machineEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply SnackMachineMoneyReturnedEvent: Snack machine {machineEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackMachineMoneyReturnedEvent: Snack machine {machineEvent.MachineId} version {machine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machine.MoneyInside = machineEvent.MoneyInside.ToProjection(machine.MoneyInside);
            machine.AmountInTransaction = machineEvent.AmountInTransaction;
            machine.LastModifiedAt = machineEvent.OperatedAt;
            machine.LastModifiedBy = machineEvent.OperatedBy;
            machine.Version = machineEvent.Version;
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
            var machine = await _dbContext.SnackMachines.Include(sm => sm.Slots).FirstOrDefaultAsync(sm => sm.Id == machineEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply SnackMachineSnacksLoadedEvent: Snack machine {machineEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackMachineSnacksLoadedEvent: Snack machine {machineEvent.MachineId} version {machine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            var slot = machine.Slots.FirstOrDefault(sl => sl.MachineId == machineEvent.Slot.MachineId && sl.Position == machineEvent.Slot.Position);
            if (slot == null)
            {
                slot = new Slot();
                machine.Slots.Add(slot);
            }
            await machineEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync, slot);
            machine.SlotsCount = machineEvent.SlotsCount;
            machine.SnackCount = machineEvent.SnackCount;
            machine.SnackQuantity = machineEvent.SnackQuantity;
            machine.SnackAmount = machineEvent.SnackAmount;
            machine.LastModifiedAt = machineEvent.OperatedAt;
            machine.LastModifiedBy = machineEvent.OperatedBy;
            machine.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackMachineMoneyReturnedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(SnackMachineSnacksUnloadedEvent machineEvent)
    {
        try
        {
            var machine = await _dbContext.SnackMachines.Include(sm => sm.Slots).FirstOrDefaultAsync(sm => sm.Id == machineEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply SnackMachineSnacksUnloadedEvent: Snack machine {machineEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackMachineSnacksUnloadedEvent: Snack machine {machineEvent.MachineId} version {machine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            var slot = machine.Slots.FirstOrDefault(sl => sl.MachineId == machineEvent.Slot.MachineId && sl.Position == machineEvent.Slot.Position);
            if (slot == null)
            {
                slot = new Slot();
                machine.Slots.Add(slot);
            }
            await machineEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync, slot);
            machine.SlotsCount = machineEvent.SlotsCount;
            machine.SnackCount = machineEvent.SnackCount;
            machine.SnackQuantity = machineEvent.SnackQuantity;
            machine.SnackAmount = machineEvent.SnackAmount;
            machine.LastModifiedAt = machineEvent.OperatedAt;
            machine.LastModifiedBy = machineEvent.OperatedBy;
            machine.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply SnackMachineSnacksUnloadedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(SnackMachineSnackBoughtEvent machineEvent)
    {
        try
        {
            var machine = await _dbContext.SnackMachines.Include(sm => sm.Slots).FirstOrDefaultAsync(sm => sm.Id == machineEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply SnackMachineSnackBoughtEvent: Snack machine {machineEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply SnackMachineSnackBoughtEvent: Snack machine {machineEvent.MachineId} version {machine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machine.AmountInTransaction = machineEvent.AmountInTransaction;
            var slot = machine.Slots.FirstOrDefault(sl => sl.MachineId == machineEvent.Slot.MachineId && sl.Position == machineEvent.Slot.Position);
            if (slot == null)
            {
                slot = new Slot();
                machine.Slots.Add(slot);
            }
            await machineEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync, slot);
            machine.SnackQuantity = machineEvent.SnackQuantity;
            machine.SnackAmount = machineEvent.SnackAmount;
            machine.LastModifiedAt = machineEvent.OperatedAt;
            machine.LastModifiedBy = machineEvent.OperatedBy;
            machine.Version = machineEvent.Version;
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
                var machineId = machineEvent.MachineId;
                var machineGrain = GrainFactory.GetGrain<ISnackMachineGrain>(machineId);
                var machineInGrain = await machineGrain.GetStateAsync();
                var machine = await _dbContext.SnackMachines.FindAsync(machineId);
                if (machineInGrain == null)
                {
                    if (machine == null)
                    {
                        return;
                    }
                    _dbContext.Remove(machine);
                    await _dbContext.SaveChangesAsync();
                    return;
                }
                if (machine == null)
                {
                    machine = new SnackMachine();
                    _dbContext.SnackMachines.Add(machine);
                }
                machine = await machineInGrain.ToProjection(GetSnackNameAndPictureUrlAsync, machine);
                machine.Version = await machineGrain.GetVersionAsync();
                var purchaseStatsGrain = GrainFactory.GetGrain<ISnackMachinePurchaseStatsGrain>(machineId);
                machine.BoughtCount = await purchaseStatsGrain.GetCountAsync();
                machine.BoughtAmount = await purchaseStatsGrain.GetAmountAsync();
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
