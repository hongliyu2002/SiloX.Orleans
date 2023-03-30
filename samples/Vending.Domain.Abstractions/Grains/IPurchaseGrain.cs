using Orleans.Concurrency;
using Orleans.FluentResults;
using Vending.Domain.Abstractions.Commands;
using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Abstractions.Grains;

/// <summary>
///     This interface defines the contract for the snack machine snack purchase grain.
/// </summary>
public interface IPurchaseGrain
{
    /// <summary>
    ///     Asynchronously retrieves the current state of the snack machine snack purchase grain.
    /// </summary>
    [AlwaysInterleave]
    Task<Purchase> GetStateAsync();

    /// <summary>
    ///     Asynchronously checks whether the snack machine snack purchase grain can be initialized with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanInitializeAsync(PurchaseInitializeCommand command);

    /// <summary>
    ///     Asynchronously initializes the snack machine snack purchase grain with the given command.
    /// </summary>
    Task<Result> InitializeAsync(PurchaseInitializeCommand command);
}
