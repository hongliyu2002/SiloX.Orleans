using Fluxera.Extensions.Common;
using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using Orleans.FluentResults;
using Orleans.Providers;
using SiloX.Domain.Abstractions;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;
using Vending.Domain.Abstractions.Purchases;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain.Machines;

[LogConsistencyProvider(ProviderName = Constants.LogConsistencyName)]
[StorageProvider(ProviderName = Constants.GrainStorageName)]
public sealed class MachineGrain : EventSourcingGrainWithGuidKey<Machine, MachineCommand, MachineEvent, MachineErrorEvent>, IMachineGrain
{
    private readonly DomainDbContext _dbContext;
    private readonly IGuidGenerator _guidGenerator;

    /// <inheritdoc />
    public MachineGrain(DomainDbContext dbContext, IGuidGenerator guidGenerator)
        : base(Constants.StreamProviderName)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _guidGenerator = Guard.Against.Null(guidGenerator, nameof(guidGenerator));
    }

    /// <inheritdoc />
    protected override string GetPubStreamNamespace()
    {
        return Constants.MachinesNamespace;
    }

    /// <inheritdoc />
    protected override string GetPubBroadcastStreamNamespace()
    {
        return Constants.MachinesBroadcastNamespace;
    }

    /// <inheritdoc />
    public Task<Machine> GetMachineAsync()
    {
        return Task.FromResult(State);
    }

    /// <inheritdoc />
    public Task<int> GetVersionAsync()
    {
        return Task.FromResult(Version);
    }

    private Result ValidateInitialize(MachineInitializeCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Machine {machineId} has already been deleted.")
                     .Verify(State.IsCreated == false, $"Machine {machineId} already exists.")
                     .Verify(command.MoneyInside != null, "Money inside should not be empty.")
                     .Verify(command.Slots.IsNotNullOrEmpty(), "Slots should not be empty.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanInitializeAsync(MachineInitializeCommand command)
    {
        return Task.FromResult(ValidateInitialize(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> InitializeAsync(MachineInitializeCommand command)
    {
        return ValidateInitialize(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .TapTryAsync(() => PublishAsync(new MachineInitializedEvent(State.Id, Version, State.MoneyInside, State.Slots, State.SlotCount, State.SnackCount, State.SnackQuantity, State.SnackAmount, command.TraceId, DateTimeOffset.UtcNow,
                                                                          command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 201, errors.ToListMessages(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateDelete(MachineDeleteCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Machine {machineId} has already been deleted.")
                     .Verify(State.IsCreated, $"Machine {machineId} is not initialized.")
                     .Verify(State.AmountInTransaction == 0m, $"Machine {machineId} still in transaction with amount {State.AmountInTransaction}.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanDeleteAsync(MachineDeleteCommand command)
    {
        return Task.FromResult(ValidateDelete(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> DeleteAsync(MachineDeleteCommand command)
    {
        return ValidateDelete(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .TapTryAsync(() => PublishAsync(new MachineDeletedEvent(State.Id, Version, State.MoneyInside, State.AmountInTransaction, State.Slots, State.SlotCount, State.SnackCount, State.SnackQuantity, State.SnackAmount, command.TraceId,
                                                                      State.DeletedAt ?? DateTimeOffset.UtcNow, State.DeletedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 202, errors.ToListMessages(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateUpdate(MachineUpdateCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Machine {machineId} has already been deleted.")
                     .Verify(State.IsCreated, $"Machine {machineId} is not initialized.")
                     .Verify(command.MoneyInside != null, "Money inside should not be empty.")
                     .Verify(command.Slots.IsNotNullOrEmpty(), "Slots should not be empty.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanUpdateAsync(MachineUpdateCommand command)
    {
        return Task.FromResult(ValidateUpdate(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> UpdateAsync(MachineUpdateCommand command)
    {
        return ValidateUpdate(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .TapTryAsync(() => PublishAsync(new MachineUpdatedEvent(State.Id, Version, State.MoneyInside, State.Slots, State.SlotCount, State.SnackCount, State.SnackQuantity, State.SnackAmount, command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 203, errors.ToListMessages(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateAddSlot(MachineAddSlotCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Machine {machineId} has already been deleted.")
                     .Verify(State.IsCreated, $"Machine {machineId} is not initialized.")
                     .Verify(State.TryGetSlot(command.Position, out _) == false, $"Slot at position {command.Position} in the machine {machineId} already exists.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanAddSlotAsync(MachineAddSlotCommand command)
    {
        return Task.FromResult(ValidateAddSlot(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> AddSlotAsync(MachineAddSlotCommand command)
    {
        return ValidateAddSlot(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .TapTryAsync(() => PublishAsync(new MachineSlotAddedEvent(State.Id, Version, State.Slots.Single(ms => ms.Position == command.Position), State.SlotCount, State.SnackCount, State.SnackQuantity, State.SnackAmount, command.TraceId,
                                                                        State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 204, errors.ToListMessages(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateDeleteSlot(MachineRemoveSlotCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Machine {machineId} has already been deleted.")
                     .Verify(State.IsCreated, $"Machine {machineId} is not initialized.")
                     .Verify(State.TryGetSlot(command.Position, out _), $"Slot at position {command.Position} in the machine {machineId} does not exist.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanRemoveSlotAsync(MachineRemoveSlotCommand command)
    {
        return Task.FromResult(ValidateDeleteSlot(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> RemoveSlotAsync(MachineRemoveSlotCommand command)
    {
        MachineSlot? slotToRemove = null;
        return ValidateDeleteSlot(command)
              .TapTry(() => State.TryGetSlot(command.Position, out slotToRemove))
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .TapTryAsync(() => PublishAsync(new MachineSlotRemovedEvent(State.Id, Version, slotToRemove!, State.SlotCount, State.SnackCount, State.SnackQuantity, State.SnackAmount, command.TraceId, State.LastModifiedAt ?? DateTimeOffset.UtcNow,
                                                                          State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 205, errors.ToListMessages(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateLoadMoney(MachineLoadMoneyCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Machine {machineId} has already been deleted.")
                     .Verify(State.IsCreated, $"Machine {machineId} is not initialized.")
                     .Verify(command.Money != Money.Zero, "Loading money should not be zero.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanLoadMoneyAsync(MachineLoadMoneyCommand command)
    {
        return Task.FromResult(ValidateLoadMoney(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> LoadMoneyAsync(MachineLoadMoneyCommand command)
    {
        return ValidateLoadMoney(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .TapTryAsync(() => PublishAsync(new MachineMoneyLoadedEvent(State.Id, Version, State.MoneyInside, command.TraceId, State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 206, errors.ToListMessages(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateUnloadMoney(MachineUnloadMoneyCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Machine {machineId} has already been deleted.")
                     .Verify(State.IsCreated, $"Machine {machineId} is not initialized.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanUnloadMoneyAsync(MachineUnloadMoneyCommand command)
    {
        return Task.FromResult(ValidateUnloadMoney(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> UnloadMoneyAsync(MachineUnloadMoneyCommand command)
    {
        return ValidateUnloadMoney(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .TapTryAsync(() => PublishAsync(new MachineMoneyUnloadedEvent(State.Id, Version, State.MoneyInside, command.TraceId, State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 207, errors.ToListMessages(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateInsertMoney(MachineInsertMoneyCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Machine {machineId} has already been deleted.")
                     .Verify(State.IsCreated, $"Machine {machineId} is not initialized.")
                     .Verify(Money.CoinsAndNotes.Contains(command.Money), $"Only single coin or note should be inserted into the machine {machineId}.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanInsertMoneyAsync(MachineInsertMoneyCommand command)
    {
        return Task.FromResult(ValidateInsertMoney(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> InsertMoneyAsync(MachineInsertMoneyCommand command)
    {
        return ValidateInsertMoney(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .TapTryAsync(() => PublishAsync(new MachineMoneyInsertedEvent(State.Id, Version, State.MoneyInside, State.AmountInTransaction, command.TraceId, State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 208, errors.ToListMessages(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateReturnMoney(MachineReturnMoneyCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Machine {machineId} has already been deleted.")
                     .Verify(State.IsCreated, $"Machine {machineId} is not initialized.")
                     .Verify(State.MoneyInside.CanAllocate(State.AmountInTransaction, out _), $"Not enough change in the machine {machineId}.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanReturnMoneyAsync(MachineReturnMoneyCommand command)
    {
        return Task.FromResult(ValidateReturnMoney(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> ReturnMoneyAsync(MachineReturnMoneyCommand command)
    {
        return ValidateReturnMoney(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .TapTryAsync(() => PublishAsync(new MachineMoneyReturnedEvent(State.Id, Version, State.MoneyInside, State.AmountInTransaction, command.TraceId, State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 209, errors.ToListMessages(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateLoadSnacks(MachineLoadSnacksCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Machine {machineId} has already been deleted.")
                     .Verify(State.IsCreated, $"Machine {machineId} is not initialized.")
                     .Verify(State.TryGetSlot(command.Position, out var slot), $"Slot at position {command.Position} in the machine {machineId} does not exist.")
                     .Verify(command.SnackPile != null, "Snack pile to load should not be empty.")
                     .Verify(command.SnackPile!.Quantity > 0, "Snack pile quantity to load should be greater than zero.")
                     .Verify(slot!.SnackPile == null || slot.SnackPile.SnackId == command.SnackPile.SnackId, $"Snack pile to load should be of the same type as the one already in the machineSlot at position {command.Position} in the machine {machineId}.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanLoadSnacksAsync(MachineLoadSnacksCommand command)
    {
        return Task.FromResult(ValidateLoadSnacks(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> LoadSnacksAsync(MachineLoadSnacksCommand command)
    {
        return ValidateLoadSnacks(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .TapTryAsync(() => PublishAsync(new MachineSnacksLoadedEvent(State.Id, Version, State.Slots.Single(ms => ms.Position == command.Position), State.SlotCount, State.SnackCount, State.SnackQuantity, State.SnackAmount, command.TraceId,
                                                                           State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 210, errors.ToListMessages(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateUnloadSnacks(MachineUnloadSnacksCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Machine {machineId} has already been deleted.")
                     .Verify(State.IsCreated, $"Machine {machineId} is not initialized.")
                     .Verify(State.TryGetSlot(command.Position, out _), $"Slot at position {command.Position} in the machine {machineId} does not exist.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanUnloadSnacksAsync(MachineUnloadSnacksCommand command)
    {
        return Task.FromResult(ValidateUnloadSnacks(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> UnloadSnacksAsync(MachineUnloadSnacksCommand command)
    {
        return ValidateUnloadSnacks(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .TapTryAsync(() => PublishAsync(new MachineSnacksUnloadedEvent(State.Id, Version, State.Slots.Single(ms => ms.Position == command.Position), State.SlotCount, State.SnackCount, State.SnackQuantity, State.SnackAmount, command.TraceId,
                                                                             State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 211, errors.ToListMessages(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateBuySnack(MachineBuySnackCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Machine {machineId} has already been deleted.")
                     .Verify(State.IsCreated, $"Machine {machineId} is not initialized.")
                     .Verify(State.TryGetSlot(command.Position, out var slot), $"Slot at position {command.Position} in the machine {machineId} does not exist.")
                     .Verify(slot?.SnackPile != null, $"Snack pile of the machineSlot at position {command.Position} in the machine {machineId} does not exist.")
                     .Verify(slot?.SnackPile != null && slot.SnackPile.CanSubtractOne(), $"Not enough snack in the snack pile of the machineSlot at position {command.Position} in the machine {machineId}.")
                     .Verify(State.AmountInTransaction >= (slot?.SnackPile?.Price ?? 0), $"Not enough money (￥{State.AmountInTransaction}) to buy the snack {slot?.SnackPile?.SnackId} (￥{slot?.SnackPile?.Price}) in the machine {machineId}.")
                     .Verify(State.MoneyInside.CanAllocate(State.AmountInTransaction - (slot?.SnackPile?.Price ?? 0), out _), $"Not enough change in the machine {machineId} after purchase.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanBuySnackAsync(MachineBuySnackCommand command)
    {
        return Task.FromResult(ValidateBuySnack(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> BuySnackAsync(MachineBuySnackCommand command)
    {
        return ValidateBuySnack(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => InitializePurchaseAsync(command))
              .MapTryIfAsync(initialized => initialized,
                             () => PublishAsync(new MachineSnackBoughtEvent(State.Id, Version, State.AmountInTransaction, State.Slots.Single(ms => ms.Position == command.Position), State.SnackQuantity, State.SnackAmount, command.TraceId,
                                                                            State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 212, errors.ToListMessages(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    #region Persistence

    private async Task PersistAsync()
    {
        var newMachine = State;
        var existingMachine = await _dbContext.Machines.Include(m => m.MoneyInside).Include(m => m.Slots).ThenInclude(ms => ms.SnackPile).Include(m => m.SnackStats).FirstOrDefaultAsync(m => m.Id == newMachine.Id);
        if (existingMachine == null)
        {
            _dbContext.Machines.Add(newMachine);
        }
        else
        {
            _dbContext.Entry(existingMachine).CurrentValues.SetValues(newMachine);
            existingMachine.MoneyInside = newMachine.MoneyInside;
            // Remove slots that are not in the new machine.
            foreach (var existingSlot in existingMachine.Slots.Where(slot => newMachine.Slots.All(ms => ms.Position != slot.Position)))
            {
                _dbContext.Remove(existingSlot);
            }
            // Update or add slots.
            foreach (var newSlot in newMachine.Slots)
            {
                var existingSlot = existingMachine.Slots.SingleOrDefault(ms => ms.Position == newSlot.Position);
                if (existingSlot == null)
                {
                    existingMachine.Slots.Add(newSlot);
                }
                else
                {
                    _dbContext.Entry(existingSlot).CurrentValues.SetValues(newSlot);
                    existingSlot.SnackPile = newSlot.SnackPile;
                }
            }
            // Remove snack stats that are not in the new machine.
            foreach (var existingSnackStat in existingMachine.SnackStats.Where(snackStat => newMachine.SnackStats.All(mss => mss.SnackId != snackStat.SnackId)))
            {
                _dbContext.Remove(existingSnackStat);
            }
            // Update or add snackStats.
            foreach (var newSnackStat in newMachine.SnackStats)
            {
                var existingSnackStat = existingMachine.SnackStats.SingleOrDefault(mss => mss.SnackId == newSnackStat.SnackId);
                if (existingSnackStat == null)
                {
                    existingMachine.SnackStats.Add(newSnackStat);
                }
                else
                {
                    _dbContext.Entry(existingSnackStat).CurrentValues.SetValues(newSnackStat);
                }
            }
            _dbContext.Entry(existingMachine).State = EntityState.Modified;
        }
        await _dbContext.SaveChangesAsync();
    }

    #endregion

    #region External Calls

    private async Task<bool> InitializePurchaseAsync(MachineBuySnackCommand command)
    {
        if (State.TryGetSlot(command.Position, out var slot) && slot?.SnackPile != null)
        {
            var purchaseId = _guidGenerator.Create();
            var purchaseGrain = GrainFactory.GetGrain<IPurchaseGrain>(purchaseId);
            var result = await purchaseGrain.InitializeAsync(new PurchaseInitializeCommand(purchaseId, State.Id, slot.Position, slot.SnackPile.SnackId, slot.SnackPile.Price, command.TraceId, command.OperatedAt, command.OperatedBy));
            return result.IsSuccess;
        }
        return false;
    }

    #endregion

}