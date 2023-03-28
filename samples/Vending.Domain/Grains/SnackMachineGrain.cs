using System.Collections.Immutable;
using Fluxera.Utilities.Extensions;
using Orleans.FluentResults;
using Orleans.Providers;
using SiloX.Domain.Abstractions;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Commands;
using Vending.Domain.Abstractions.Events;
using Vending.Domain.Abstractions.Grains;
using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Grains;

[LogConsistencyProvider(ProviderName = Constants.LogConsistencyName2)]
[StorageProvider(ProviderName = Constants.GrainStorageName2)]
public sealed class SnackMachineGrain
    : EventSourcingGrain<SnackMachine, SnackMachineEvent, SnackMachineErrorEvent>,
      ISnackMachineGrain
{
    /// <inheritdoc />
    public SnackMachineGrain()
        : base(Constants.StreamProviderName2)
    {
    }

    /// <inheritdoc />
    protected override string GetStreamNamespace()
    {
        return Constants.SnackMachinesNamespace;
    }

    /// <inheritdoc />
    protected override Task<bool> PersistAsync(SnackMachineEvent domainEvent)
    {
        return Task.FromResult(true);
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
        var id = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {id} has already been removed.")
                     .Verify(State.IsCreated == false, $"Snack machine {id} already exists.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace, "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanInitializeAsync(SnackMachineInitializeCommand command)
    {
        return Task.FromResult(ValidateInitialize(command)
                                  .IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> InitializeAsync(SnackMachineInitializeCommand command)
    {
        var id = this.GetPrimaryKey();
        return ValidateInitialize(command)
              .TapErrorTryAsync(errors => PublishOnErrorAsync(new SnackMachineErrorEvent(id, Version, 201, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .TapTryAsync(() => PublishOnPersistedAsync(new SnackMachineInitializedEvent(id, Version, command.MoneyInside, command.Slots, command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateRemove(SnackMachineRemoveCommand command)
    {
        var id = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {id} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {id} is not initialized.")
                     .Verify(State.AmountInTransaction == 0m, $"Snack machine {id} still in transaction with amount {State.AmountInTransaction}.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace, "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanRemoveAsync(SnackMachineRemoveCommand command)
    {
        return Task.FromResult(ValidateRemove(command)
                                  .IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> RemoveAsync(SnackMachineRemoveCommand command)
    {
        var id = this.GetPrimaryKey();
        return ValidateRemove(command)
              .TapErrorTryAsync(errors => PublishOnErrorAsync(new SnackMachineErrorEvent(id, Version, 202, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .TapTryAsync(() => PublishOnPersistedAsync(new SnackMachineRemovedEvent(id, Version, command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateLoadMoney(SnackMachineLoadMoneyCommand command)
    {
        var id = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {id} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {id} is not initialized.")
                     .Verify(command.Money != Money.Zero, "Loading money should not be zero.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace, "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanLoadMoneyAsync(SnackMachineLoadMoneyCommand command)
    {
        return Task.FromResult(ValidateLoadMoney(command)
                                  .IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> LoadMoneyAsync(SnackMachineLoadMoneyCommand command)
    {
        var id = this.GetPrimaryKey();
        return ValidateLoadMoney(command)
              .TapErrorTryAsync(errors => PublishOnErrorAsync(new SnackMachineErrorEvent(id, Version, 203, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .TapTryAsync(() =>
                           {
                               var moneyInside = State.MoneyInside + command.Money;
                               return PublishOnPersistedAsync(new SnackMachineMoneyLoadedEvent(id, Version, moneyInside, command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy));
                           });
    }

    private Result ValidateUnloadMoney(SnackMachineUnloadMoneyCommand command)
    {
        var id = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {id} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {id} is not initialized.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace, "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanUnloadMoneyAsync(SnackMachineUnloadMoneyCommand command)
    {
        return Task.FromResult(ValidateUnloadMoney(command)
                                  .IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> UnloadMoneyAsync(SnackMachineUnloadMoneyCommand command)
    {
        var id = this.GetPrimaryKey();
        return ValidateUnloadMoney(command)
              .TapErrorTryAsync(errors => PublishOnErrorAsync(new SnackMachineErrorEvent(id, Version, 204, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .TapTryAsync(() => PublishOnPersistedAsync(new SnackMachineMoneyUnloadedEvent(id, Version, Money.Zero, command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateInsertMoney(SnackMachineInsertMoneyCommand command)
    {
        var id = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {id} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {id} is not initialized.")
                     .Verify(Money.CoinsAndNotes.Contains(command.Money), $"Only single coin or note should be inserted into the snack machine {id}.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace, "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanInsertMoneyAsync(SnackMachineInsertMoneyCommand command)
    {
        return Task.FromResult(ValidateInsertMoney(command)
                                  .IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> InsertMoneyAsync(SnackMachineInsertMoneyCommand command)
    {
        var id = this.GetPrimaryKey();
        return ValidateInsertMoney(command)
              .TapErrorTryAsync(errors => PublishOnErrorAsync(new SnackMachineErrorEvent(id, Version, 205, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .TapTryAsync(() =>
                           {
                               var moneyInside = State.MoneyInside + command.Money;
                               var amountInTransaction = State.AmountInTransaction + command.Money.Amount;
                               return PublishOnPersistedAsync(new SnackMachineMoneyInsertedEvent(id, Version, moneyInside, amountInTransaction, command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy));
                           });
    }

    private Result ValidateReturnMoney(SnackMachineReturnMoneyCommand command)
    {
        var id = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {id} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {id} is not initialized.")
                     .Verify(State.MoneyInside.CanAllocate(State.AmountInTransaction, out _), $"Not enough change in the snack machine {id}.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace, "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanReturnMoneyAsync(SnackMachineReturnMoneyCommand command)
    {
        return Task.FromResult(ValidateReturnMoney(command)
                                  .IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> ReturnMoneyAsync(SnackMachineReturnMoneyCommand command)
    {
        var id = this.GetPrimaryKey();
        var moneyReturned = Money.Zero;
        return ValidateReturnMoney(command)
              .TapErrorTryAsync(errors => PublishOnErrorAsync(new SnackMachineErrorEvent(id, Version, 206, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .TapTryIfAsync(() => State.MoneyInside.CanAllocate(State.AmountInTransaction, out moneyReturned), () =>
                                                                                                                {
                                                                                                                    var moneyInside = State.MoneyInside - moneyReturned;
                                                                                                                    return PublishOnPersistedAsync(new SnackMachineMoneyReturnedEvent(id, Version, moneyInside, 0, command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy));
                                                                                                                });
    }

    private Result ValidateLoadSnacks(SnackMachineLoadSnacksCommand command)
    {
        var id = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {id} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {id} is not initialized.")
                     .Verify(State.TryGetSlot(command.Position, out _), $"Slot at position {command.Position} in the snack machine {id} does not exist.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace, "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanLoadSnacksAsync(SnackMachineLoadSnacksCommand command)
    {
        return Task.FromResult(ValidateLoadSnacks(command)
                                  .IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> LoadSnacksAsync(SnackMachineLoadSnacksCommand command)
    {
        var id = this.GetPrimaryKey();
        return ValidateLoadSnacks(command)
              .TapErrorTryAsync(errors => PublishOnErrorAsync(new SnackMachineErrorEvent(id, Version, 207, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .TapTryAsync(() =>
                           {
                               var slot = new Slot(State.Id, command.Position, command.SnackPile);
                               return PublishOnPersistedAsync(new SnackMachineSnacksLoadedEvent(id, Version,slot, command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy));
                           });
    }

    private Result ValidateBuySnack(SnackMachineBuySnackCommand command)
    {
        var id = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {id} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {id} is not initialized.")
                     .Verify(State.TryGetSlot(command.Position, out var slot), $"Slot at position {command.Position} in the snack machine {id} does not exist.")
                     .Verify(slot?.SnackPile != null, $"Snack pile of the slot at position {command.Position} in the snack machine {id} does not exist.")
                     .Verify(slot?.SnackPile != null && slot.SnackPile.CanPopOne(out _), $"Not enough snack in the snack pile of the slot at position {command.Position} in the snack machine {id}.")
                     .Verify(State.AmountInTransaction >= (slot?.SnackPile?.Price ?? 0), $"Not enough money (￥{State.AmountInTransaction}) to buy the snack {slot?.SnackPile?.SnackId} (￥{slot?.SnackPile?.Price}) in the snack machine {id}.")
                     .Verify(State.MoneyInside.CanAllocate(State.AmountInTransaction - (slot?.SnackPile?.Price ?? 0), out _), $"Not enough change in the snack machine {id} after purchase.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace, "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanBuySnackAsync(SnackMachineBuySnackCommand command)
    {
        return Task.FromResult(ValidateBuySnack(command)
                                  .IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> BuySnackAsync(SnackMachineBuySnackCommand command)
    {
        var id = this.GetPrimaryKey();
        Slot? slot = null;
        return ValidateBuySnack(command)
              .TapErrorTryAsync(errors => PublishOnErrorAsync(new SnackMachineErrorEvent(id, Version, 208, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .TapTryIfAsync(() => State.TryGetSlot(command.Position, out slot), () =>
                                                                                 {
                                                                                     slot!.SnackPile = slot.SnackPile! with { Quantity = slot.SnackPile.Quantity - 1 };
                                                                                     var amountInTransaction = State.AmountInTransaction - slot.SnackPile.Price;
                                                                                     return PublishOnPersistedAsync(new SnackMachineSnackBoughtEvent(id, Version, slot, amountInTransaction, command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy));
                                                                                 });
    }
}
