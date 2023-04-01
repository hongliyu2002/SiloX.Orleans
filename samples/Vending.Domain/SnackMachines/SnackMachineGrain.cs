using System.Collections.Immutable;
using Fluxera.Extensions.Common;
using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using Orleans.FluentResults;
using Orleans.Providers;
using SiloX.Domain.Abstractions;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Purchases;
using Vending.Domain.Abstractions.SnackMachines;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain.SnackMachines;

[LogConsistencyProvider(ProviderName = Constants.LogConsistencyName)]
[StorageProvider(ProviderName = Constants.GrainStorageName)]
public sealed class SnackMachineGrain : EventSourcingGrainWithGuidKey<SnackMachine, SnackMachineCommand, SnackMachineEvent, SnackMachineErrorEvent>, ISnackMachineGrain
{
    private readonly DomainDbContext _dbContext;
    private readonly IGuidGenerator _guidGenerator;

    /// <inheritdoc />
    public SnackMachineGrain(DomainDbContext dbContext, IGuidGenerator guidGenerator)
        : base(Constants.StreamProviderName)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _guidGenerator = Guard.Against.Null(guidGenerator, nameof(guidGenerator));
    }

    /// <inheritdoc />
    protected override string GetStreamNamespace()
    {
        return Constants.SnackMachinesNamespace;
    }

    /// <inheritdoc />
    protected override string GetBroadcastStreamNamespace()
    {
        return Constants.SnackMachinesBroadcastNamespace;
    }

    /// <inheritdoc />
    public Task<SnackMachine> GetStateAsync()
    {
        return Task.FromResult(State);
    }

    /// <inheritdoc />
    public Task<int> GetVersionAsync()
    {
        return Task.FromResult(Version);
    }

    /// <inheritdoc />
    public Task<Money> GetMoneyInsideAsync()
    {
        return Task.FromResult(State.MoneyInside);
    }

    /// <inheritdoc />
    public Task<decimal> GetAmountInTransactionAsync()
    {
        return Task.FromResult(State.AmountInTransaction);
    }

    /// <inheritdoc />
    public Task<ImmutableList<Slot>> GetSlotsAsync()
    {
        return Task.FromResult(State.Slots.ToImmutableList());
    }

    private Result ValidateInitialize(SnackMachineInitializeCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated == false, $"Snack machine {machineId} already exists.")
                     .Verify(command.MoneyInside != null, "Money inside should not be empty.")
                     .Verify(command.Slots.IsNotNullOrEmpty(), "Slots should not be empty.")
                     .Verify(command.Slots.All(slot => slot.MachineId == machineId), $"Slots should be owned by the same snack machine {machineId}.")
                     .Verify(command.Slots.GroupBy(slot => new { slot.MachineId, slot.Position }).Any(group => group.Count() == 1), "Slots should not contain duplicate positions.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanInitializeAsync(SnackMachineInitializeCommand command)
    {
        return Task.FromResult(ValidateInitialize(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> InitializeAsync(SnackMachineInitializeCommand command)
    {
        return ValidateInitialize(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => PublishAsync(new SnackMachineInitializedEvent(State.Id, Version, State.MoneyInside, State.Slots.ToImmutableList(), State.SlotsCount, State.SnackCount, State.SnackQuantity, State.SnackAmount, command.TraceId,
                                                                               DateTimeOffset.UtcNow, command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackMachineErrorEvent(this.GetPrimaryKey(), Version, 201, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateRemove(SnackMachineRemoveCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {machineId} is not initialized.")
                     .Verify(State.AmountInTransaction == 0m, $"Snack machine {machineId} still in transaction with amount {State.AmountInTransaction}.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanRemoveAsync(SnackMachineRemoveCommand command)
    {
        return Task.FromResult(ValidateRemove(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> RemoveAsync(SnackMachineRemoveCommand command)
    {
        return ValidateRemove(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => PublishAsync(new SnackMachineRemovedEvent(State.Id, Version, State.MoneyInside, State.AmountInTransaction, State.Slots.ToImmutableList(), State.SlotsCount, State.SnackCount, State.SnackQuantity, State.SnackAmount,
                                                                           command.TraceId, State.DeletedAt ?? DateTimeOffset.UtcNow, State.DeletedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackMachineErrorEvent(this.GetPrimaryKey(), Version, 202, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateLoadMoney(SnackMachineLoadMoneyCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {machineId} is not initialized.")
                     .Verify(command.Money != Money.Zero, "Loading money should not be zero.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanLoadMoneyAsync(SnackMachineLoadMoneyCommand command)
    {
        return Task.FromResult(ValidateLoadMoney(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> LoadMoneyAsync(SnackMachineLoadMoneyCommand command)
    {
        return ValidateLoadMoney(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => PublishAsync(new SnackMachineMoneyLoadedEvent(State.Id, Version, State.MoneyInside, command.TraceId, State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackMachineErrorEvent(this.GetPrimaryKey(), Version, 203, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateUnloadMoney(SnackMachineUnloadMoneyCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {machineId} is not initialized.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanUnloadMoneyAsync(SnackMachineUnloadMoneyCommand command)
    {
        return Task.FromResult(ValidateUnloadMoney(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> UnloadMoneyAsync(SnackMachineUnloadMoneyCommand command)
    {
        return ValidateUnloadMoney(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => PublishAsync(new SnackMachineMoneyUnloadedEvent(State.Id, Version, State.MoneyInside, command.TraceId, State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackMachineErrorEvent(this.GetPrimaryKey(), Version, 204, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateInsertMoney(SnackMachineInsertMoneyCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {machineId} is not initialized.")
                     .Verify(Money.CoinsAndNotes.Contains(command.Money), $"Only single coin or note should be inserted into the snack machine {machineId}.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanInsertMoneyAsync(SnackMachineInsertMoneyCommand command)
    {
        return Task.FromResult(ValidateInsertMoney(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> InsertMoneyAsync(SnackMachineInsertMoneyCommand command)
    {
        return ValidateInsertMoney(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => PublishAsync(new SnackMachineMoneyInsertedEvent(State.Id, Version, State.MoneyInside, State.AmountInTransaction, command.TraceId, State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackMachineErrorEvent(this.GetPrimaryKey(), Version, 205, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateReturnMoney(SnackMachineReturnMoneyCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {machineId} is not initialized.")
                     .Verify(State.MoneyInside.CanAllocate(State.AmountInTransaction, out _), $"Not enough change in the snack machine {machineId}.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanReturnMoneyAsync(SnackMachineReturnMoneyCommand command)
    {
        return Task.FromResult(ValidateReturnMoney(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> ReturnMoneyAsync(SnackMachineReturnMoneyCommand command)
    {
        return ValidateReturnMoney(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => PublishAsync(new SnackMachineMoneyReturnedEvent(State.Id, Version, State.MoneyInside, State.AmountInTransaction, command.TraceId, State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackMachineErrorEvent(this.GetPrimaryKey(), Version, 206, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateLoadSnacks(SnackMachineLoadSnacksCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {machineId} is not initialized.")
                     .Verify(State.TryGetSlot(command.Position, out var slot), $"Slot at position {command.Position} in the snack machine {machineId} does not exist.")
                     .Verify(command.SnackPile != null, "Snack pile to load should not be empty.")
                     .Verify(command.SnackPile!.Quantity > 0, "Snack pile quantity to load should be greater than zero.")
                     .Verify(slot!.SnackPile == null || slot.SnackPile.SnackId == command.SnackPile.SnackId, $"Snack pile to load should be of the same type as the one already in the slot at position {command.Position} in the snack machine {machineId}.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanLoadSnacksAsync(SnackMachineLoadSnacksCommand command)
    {
        return Task.FromResult(ValidateLoadSnacks(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> LoadSnacksAsync(SnackMachineLoadSnacksCommand command)
    {
        return ValidateLoadSnacks(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => PublishAsync(new SnackMachineSnacksLoadedEvent(State.Id, Version, State.Slots.Single(sl => sl.Position == command.Position), State.SlotsCount, State.SnackCount, State.SnackQuantity, State.SnackAmount, command.TraceId,
                                                                                State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackMachineErrorEvent(this.GetPrimaryKey(), Version, 207, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateUnloadSnacks(SnackMachineUnloadSnacksCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {machineId} is not initialized.")
                     .Verify(State.TryGetSlot(command.Position, out _), $"Slot at position {command.Position} in the snack machine {machineId} does not exist.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanUnloadSnacksAsync(SnackMachineUnloadSnacksCommand command)
    {
        return Task.FromResult(ValidateUnloadSnacks(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> UnloadSnacksAsync(SnackMachineUnloadSnacksCommand command)
    {
        return ValidateUnloadSnacks(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => PublishAsync(new SnackMachineSnacksUnloadedEvent(State.Id, Version, State.Slots.Single(sl => sl.Position == command.Position), State.SlotsCount, State.SnackCount, State.SnackQuantity, State.SnackAmount, command.TraceId,
                                                                                  State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackMachineErrorEvent(this.GetPrimaryKey(), Version, 208, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateBuySnack(SnackMachineBuySnackCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {machineId} is not initialized.")
                     .Verify(State.TryGetSlot(command.Position, out var slot), $"Slot at position {command.Position} in the snack machine {machineId} does not exist.")
                     .Verify(slot?.SnackPile != null, $"Snack pile of the slot at position {command.Position} in the snack machine {machineId} does not exist.")
                     .Verify(slot?.SnackPile != null && slot.SnackPile.CanPopOne(), $"Not enough snack in the snack pile of the slot at position {command.Position} in the snack machine {machineId}.")
                     .Verify(State.AmountInTransaction >= (slot?.SnackPile?.Price ?? 0), $"Not enough money (￥{State.AmountInTransaction}) to buy the snack {slot?.SnackPile?.SnackId} (￥{slot?.SnackPile?.Price}) in the snack machine {machineId}.")
                     .Verify(State.MoneyInside.CanAllocate(State.AmountInTransaction - (slot?.SnackPile?.Price ?? 0), out _), $"Not enough change in the snack machine {machineId} after purchase.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanBuySnackAsync(SnackMachineBuySnackCommand command)
    {
        return Task.FromResult(ValidateBuySnack(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> BuySnackAsync(SnackMachineBuySnackCommand command)
    {
        return ValidateBuySnack(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => InitializePurchaseAsync(command))
              .MapTryIfAsync(initialized => initialized,
                             () => PublishAsync(new SnackMachineSnackBoughtEvent(State.Id, Version, State.AmountInTransaction, State.Slots.Single(sl => sl.Position == command.Position), State.SnackQuantity, State.SnackAmount, command.TraceId,
                                                                                 State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackMachineErrorEvent(this.GetPrimaryKey(), Version, 209, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    #region Persistence

    private async Task PersistAsync()
    {
        var machineInGrain = State;
        var machine = await _dbContext.SnackMachines.FindAsync(machineInGrain.Id);
        if (machine == null)
        {
            machine = new SnackMachine();
            _dbContext.SnackMachines.Add(machine);
        }
        machine.Id = machineInGrain.Id;
        machine.CreatedAt = machineInGrain.CreatedAt;
        machine.LastModifiedAt = machineInGrain.LastModifiedAt;
        machine.DeletedAt = machineInGrain.DeletedAt;
        machine.CreatedBy = machineInGrain.CreatedBy;
        machine.LastModifiedBy = machineInGrain.LastModifiedBy;
        machine.DeletedBy = machineInGrain.DeletedBy;
        machine.IsDeleted = machineInGrain.IsDeleted;
        machine.MoneyInside = machineInGrain.MoneyInside;
        machine.AmountInTransaction = machineInGrain.AmountInTransaction;
        machine.Slots = machineInGrain.Slots;
        await _dbContext.SaveChangesAsync();
    }

    #endregion

    #region External Calls

    private async Task<bool> InitializePurchaseAsync(SnackMachineBuySnackCommand command)
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
