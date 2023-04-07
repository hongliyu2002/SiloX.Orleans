using System.Collections.Immutable;
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
    protected override string GetStreamNamespace()
    {
        return Constants.MachinesNamespace;
    }

    /// <inheritdoc />
    protected override string GetBroadcastStreamNamespace()
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
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated == false, $"Snack machine {machineId} already exists.")
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
              .MapTryAsync(() => PublishAsync(new MachineInitializedEvent(State.Id, Version, State.MoneyInside, State.Slots.ToImmutableList(), State.SlotsCount, State.SnackCount, State.SnackQuantity, State.SnackAmount, command.TraceId, DateTimeOffset.UtcNow,
                                                                          command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 201, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateRemove(MachineRemoveCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {machineId} is not initialized.")
                     .Verify(State.AmountInTransaction == 0m, $"Snack machine {machineId} still in transaction with amount {State.AmountInTransaction}.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanRemoveAsync(MachineRemoveCommand command)
    {
        return Task.FromResult(ValidateRemove(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> RemoveAsync(MachineRemoveCommand command)
    {
        return ValidateRemove(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => PublishAsync(new MachineRemovedEvent(State.Id, Version, State.MoneyInside, State.AmountInTransaction, State.Slots.ToImmutableList(), State.SlotsCount, State.SnackCount, State.SnackQuantity, State.SnackAmount, command.TraceId,
                                                                      State.DeletedAt ?? DateTimeOffset.UtcNow, State.DeletedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 202, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateLoadMoney(MachineLoadMoneyCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {machineId} is not initialized.")
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
              .MapTryAsync(() => PublishAsync(new MachineMoneyLoadedEvent(State.Id, Version, State.MoneyInside, command.TraceId, State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 203, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateUnloadMoney(MachineUnloadMoneyCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {machineId} is not initialized.")
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
              .MapTryAsync(() => PublishAsync(new MachineMoneyUnloadedEvent(State.Id, Version, State.MoneyInside, command.TraceId, State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 204, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateInsertMoney(MachineInsertMoneyCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {machineId} is not initialized.")
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
              .MapTryAsync(() => PublishAsync(new MachineMoneyInsertedEvent(State.Id, Version, State.MoneyInside, State.AmountInTransaction, command.TraceId, State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 205, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateReturnMoney(MachineReturnMoneyCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {machineId} is not initialized.")
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
              .MapTryAsync(() => PublishAsync(new MachineMoneyReturnedEvent(State.Id, Version, State.MoneyInside, State.AmountInTransaction, command.TraceId, State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 206, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateLoadSnacks(MachineLoadSnacksCommand snacksCommand)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {machineId} is not initialized.")
                     .Verify(State.TryGetSlot(snacksCommand.Position, out var slot), $"Slot at position {snacksCommand.Position} in the machine {machineId} does not exist.")
                     .Verify(snacksCommand.SnackPile != null, "Snack pile to load should not be empty.")
                     .Verify(snacksCommand.SnackPile!.Quantity > 0, "Snack pile quantity to load should be greater than zero.")
                     .Verify(slot!.SnackPile == null || slot.SnackPile.SnackId == snacksCommand.SnackPile.SnackId,
                             $"Snack pile to load should be of the same type as the one already in the machineSlot at position {snacksCommand.Position} in the machine {machineId}.")
                     .Verify(snacksCommand.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanLoadSnacksAsync(MachineLoadSnacksCommand snacksCommand)
    {
        return Task.FromResult(ValidateLoadSnacks(snacksCommand).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> LoadSnacksAsync(MachineLoadSnacksCommand snacksCommand)
    {
        return ValidateLoadSnacks(snacksCommand)
              .MapTryAsync(() => RaiseConditionalEvent(snacksCommand))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => PublishAsync(new MachineSnacksLoadedEvent(State.Id, Version, State.Slots.Single(ms => ms.Position == snacksCommand.Position), State.SlotsCount, State.SnackCount, State.SnackQuantity, State.SnackAmount, snacksCommand.TraceId,
                                                                           State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? snacksCommand.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 207, errors.ToReasonStrings(), snacksCommand.TraceId, DateTimeOffset.UtcNow, snacksCommand.OperatedBy)));
    }

    private Result ValidateUnloadSnacks(MachineUnloadSnacksCommand snacksCommand)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {machineId} is not initialized.")
                     .Verify(State.TryGetSlot(snacksCommand.Position, out _), $"Slot at position {snacksCommand.Position} in the machine {machineId} does not exist.")
                     .Verify(snacksCommand.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanUnloadSnacksAsync(MachineUnloadSnacksCommand snacksCommand)
    {
        return Task.FromResult(ValidateUnloadSnacks(snacksCommand).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> UnloadSnacksAsync(MachineUnloadSnacksCommand snacksCommand)
    {
        return ValidateUnloadSnacks(snacksCommand)
              .MapTryAsync(() => RaiseConditionalEvent(snacksCommand))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => PublishAsync(new MachineSnacksUnloadedEvent(State.Id, Version, State.Slots.Single(ms => ms.Position == snacksCommand.Position), State.SlotsCount, State.SnackCount, State.SnackQuantity, State.SnackAmount, snacksCommand.TraceId,
                                                                             State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? snacksCommand.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 208, errors.ToReasonStrings(), snacksCommand.TraceId, DateTimeOffset.UtcNow, snacksCommand.OperatedBy)));
    }

    private Result ValidateBuySnack(MachineBuySnackCommand snackCommand)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack machine {machineId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack machine {machineId} is not initialized.")
                     .Verify(State.TryGetSlot(snackCommand.Position, out var slot), $"Slot at position {snackCommand.Position} in the machine {machineId} does not exist.")
                     .Verify(slot?.SnackPile != null, $"Snack pile of the machineSlot at position {snackCommand.Position} in the machine {machineId} does not exist.")
                     .Verify(slot?.SnackPile != null && slot.SnackPile.CanSubtractOne(), $"Not enough snack in the snack pile of the machineSlot at position {snackCommand.Position} in the machine {machineId}.")
                     .Verify(State.AmountInTransaction >= (slot?.SnackPile?.Price ?? 0), $"Not enough money (￥{State.AmountInTransaction}) to buy the snack {slot?.SnackPile?.SnackId} (￥{slot?.SnackPile?.Price}) in the machine {machineId}.")
                     .Verify(State.MoneyInside.CanAllocate(State.AmountInTransaction - (slot?.SnackPile?.Price ?? 0), out _), $"Not enough change in the machine {machineId} after purchase.")
                     .Verify(snackCommand.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanBuySnackAsync(MachineBuySnackCommand snackCommand)
    {
        return Task.FromResult(ValidateBuySnack(snackCommand).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> BuySnackAsync(MachineBuySnackCommand snackCommand)
    {
        return ValidateBuySnack(snackCommand)
              .MapTryAsync(() => RaiseConditionalEvent(snackCommand))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => InitializePurchaseAsync(snackCommand))
              .MapTryIfAsync(initialized => initialized,
                             () => PublishAsync(new MachineSnackBoughtEvent(State.Id, Version, State.AmountInTransaction, State.Slots.Single(ms => ms.Position == snackCommand.Position), State.SnackQuantity, State.SnackAmount, snackCommand.TraceId,
                                                                            State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? snackCommand.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(this.GetPrimaryKey(), Version, 209, errors.ToReasonStrings(), snackCommand.TraceId, DateTimeOffset.UtcNow, snackCommand.OperatedBy)));
    }

    #region Persistence

    private async Task PersistAsync()
    {
        var machine = await _dbContext.Machines.Include(m => m.Slots).Include(m => m.SnackStats).FirstOrDefaultAsync(m => m.Id == State.Id);
        if (machine != null)
        {
            _dbContext.Entry(machine).CurrentValues.SetValues(State);
        }
        else
        {
            _dbContext.Machines.Add(State);
        }
        await _dbContext.SaveChangesAsync();
    }

    #endregion

    #region External Calls

    private async Task<bool> InitializePurchaseAsync(MachineBuySnackCommand snackCommand)
    {
        if (State.TryGetSlot(snackCommand.Position, out var slot) && slot?.SnackPile != null)
        {
            var purchaseId = _guidGenerator.Create();
            var purchaseGrain = GrainFactory.GetGrain<IPurchaseGrain>(purchaseId);
            var result = await purchaseGrain.InitializeAsync(new PurchaseInitializeCommand(purchaseId, State.Id, slot.Position, slot.SnackPile.SnackId, slot.SnackPile.Price, snackCommand.TraceId, snackCommand.OperatedAt, snackCommand.OperatedBy));
            return result.IsSuccess;
        }
        return false;
    }

    #endregion

}
