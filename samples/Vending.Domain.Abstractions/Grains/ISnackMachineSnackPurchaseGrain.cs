using Orleans.Concurrency;
using Orleans.FluentResults;
using Vending.Domain.Abstractions.Commands;
using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Abstractions.Grains;

/// <summary>
///     This interface defines the contract for the snack machine snack purchase grain.
/// </summary>
public interface ISnackMachineSnackPurchaseGrain : IGrainWithStringKey
{
    /// <summary>
    ///     Asynchronously retrieves the current state of the snack machine snack purchase grain.
    /// </summary>
    [AlwaysInterleave]
    Task<SnackMachineSnackPurchase> GetStateAsync();

    /// <summary>
    ///     Asynchronously checks whether the snack machine snack purchase grain can be initialized with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanInitializeAsync(SnackMachineSnackPurchaseInitializeCommand command);

    /// <summary>
    ///     Asynchronously initializes the snack machine snack purchase grain with the given command.
    /// </summary>
    Task<Result> InitializeAsync(SnackMachineSnackPurchaseInitializeCommand command);
}
