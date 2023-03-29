using Orleans.Concurrency;
using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Abstractions.Grains;

/// <summary>
///     Grain interface IPurchaseStatsBySnackMachineGrain
/// </summary>
public interface IPurchaseStatsBySnackMachineGrain : IGrainWithGuidKey
{
    /// <summary>
    ///     Asynchronously retrieves the current state of the snack machine.
    /// </summary>
    [AlwaysInterleave]
    Task<PurchaseStats> GetStateAsync();

    /// <summary>
    ///     Asynchronously retrieves the count of purchases that have this snack machine.
    /// </summary>
    /// <returns>The count of purchases that have this snack.</returns>
    [AlwaysInterleave]
    Task<int> GetCountAsync();

    /// <summary>
    ///     Asynchronously increments the count of purchases that have this snack machine.
    /// </summary>
    /// <param name="number">The number of purchases that have this snack </param>
    Task IncrementCountAsync(int number);

    /// <summary>
    ///     Asynchronously decrements the count of purchases that have this snack machine.
    /// </summary>
    /// <param name="number">The number of purchases that have this snack </param>
    Task DecrementCountAsync(int number);

    /// <summary>
    ///     Asynchronously retrieves the amount of purchase that have this snack machine.
    /// </summary>
    /// <returns>The amount of purchases that have this snack.</returns>
    [AlwaysInterleave]
    Task<decimal> GetAmountAsync();

    /// <summary>
    ///     Asynchronously increments the amount of purchases that have this snack machine.
    /// </summary>
    /// <param name="amount">The number of purchases made for this snack</param>
    Task IncrementAmountAsync(decimal amount);

    /// <summary>
    ///     Asynchronously decrements the amount of purchases that have this snack machine.
    /// </summary>
    /// <param name="amount">The number of purchases made for this snack</param>
    Task DecrementAmountAsync(decimal amount);
}
