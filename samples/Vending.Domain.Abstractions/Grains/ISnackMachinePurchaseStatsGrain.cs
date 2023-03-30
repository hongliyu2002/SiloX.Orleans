using Orleans.Concurrency;
using Orleans.FluentResults;
using Vending.Domain.Abstractions.Commands;
using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Abstractions.Grains;

/// <summary>
///     Grain interface ISnackMachinePurchaseStatsGrain
/// </summary>
public interface ISnackMachinePurchaseStatsGrain : IGrainWithGuidKey
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
    /// <param name="command">The number of purchases that have this snack </param>
    Task<Result> IncrementCountAsync(SnackMachineIncrementBoughtCountCommand command);

    /// <summary>
    ///     Asynchronously decrements the count of purchases that have this snack machine.
    /// </summary>
    /// <param name="command">The number of purchases that have this snack </param>
    Task<Result> DecrementCountAsync(SnackMachineDecrementBoughtCountCommand command);

    /// <summary>
    ///     Asynchronously retrieves the amount of purchase that have this snack machine.
    /// </summary>
    /// <returns>The amount of purchases that have this snack.</returns>
    [AlwaysInterleave]
    Task<decimal> GetAmountAsync();

    /// <summary>
    ///     Asynchronously increments the amount of purchases that have this snack machine.
    /// </summary>
    /// <param name="command">The number of purchases made for this snack</param>
    Task<Result> IncrementAmountAsync(SnackMachineIncrementBoughtAmountCommand command);

    /// <summary>
    ///     Asynchronously decrements the amount of purchases that have this snack machine.
    /// </summary>
    /// <param name="command">The number of purchases made for this snack</param>
    Task<Result> DecrementAmountAsync(SnackMachineDecrementBoughtAmountCommand command);
}
