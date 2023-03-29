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

/// <summary>
///     Represents a grain that manages the state of a snack purchase of a snack machine.
/// </summary>
[StorageProvider(ProviderName = Constants.GrainStorageName2)]
public sealed class PurchaseGrain : StatefulGrain<Purchase, PurchaseEvent, PurchaseErrorEvent>, IPurchaseGrain
{
    /// <inheritdoc />
    public PurchaseGrain() : base(Constants.StreamProviderName2)
    {
    }

    /// <inheritdoc />
    protected override string GetPublishStreamNamespace()
    {
        return Constants.SnackMachineSnackPurchasesNamespace;
    }

    /// <inheritdoc />
    public Task<Purchase> GetStateAsync()
    {
        return Task.FromResult(State);
    }

    private Result ValidateInitialize(PurchaseInitializeCommand command)
    {
        var id = this.GetPrimaryKeyString();
        return Result.Ok()
                     .Verify(State.IsInitialized == false, $"Bought {id} is already initialized.")
                     .Verify($"{command.MachineId}:{command.Position}:{command.SnackId}" == id, $"Bought {id} is not owned by the same snack machine {command.MachineId} and slot {command.Position} and snack {command.SnackId}.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanInitializeAsync(PurchaseInitializeCommand command)
    {
        return Task.FromResult(ValidateInitialize(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> InitializeAsync(PurchaseInitializeCommand command)
    {
        var id = this.GetPrimaryKeyString();
        return ValidateInitialize(command)
              .TapErrorTryAsync(errors => PublishErrorAsync(new PurchaseErrorEvent(id, 0, 1001, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .MapTryAsync(() => ApplyAsync(command))
              .MapTryAsync(() => PublishAsync(new PurchaseInitializedEvent(id, 0, State.MachineId, State.Position, State.SnackId, State.BoughtPrice, command.TraceId, State.BoughtAt ?? DateTimeOffset.UtcNow, State.BoughtBy ?? command.OperatedBy)));
    }

    private Task ApplyAsync(PurchaseInitializeCommand command)
    {
        State.MachineId = command.MachineId;
        State.Position = command.Position;
        State.SnackId = command.SnackId;
        State.BoughtPrice = command.BoughtPrice;
        State.BoughtAt = command.OperatedAt;
        State.BoughtBy = command.OperatedBy;
        return WriteStateAsync();
    }
}
