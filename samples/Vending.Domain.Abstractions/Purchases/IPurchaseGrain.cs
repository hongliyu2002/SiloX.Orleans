using Orleans.Concurrency;
using Orleans.FluentResults;

namespace Vending.Domain.Abstractions.Purchases;

/// <summary>
///     This interface defines the contract for the machine snack purchase grain.
/// </summary>
public interface IPurchaseGrain : IGrainWithGuidKey
{
    /// <summary>
    ///     Asynchronously retrieves the current state of the machine snack purchase grain.
    /// </summary>
    [AlwaysInterleave]
    Task<Purchase> GetStateAsync();

    /// <summary>
    ///     Asynchronously checks whether the machine snack purchase grain can be initialized with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanInitializeAsync(PurchaseInitializeCommand command);

    /// <summary>
    ///     Asynchronously initializes the machine snack purchase grain with the given command.
    /// </summary>
    Task<Result> InitializeAsync(PurchaseInitializeCommand command);
}
