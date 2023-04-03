using Orleans.Concurrency;
using Orleans.FluentResults;
using Vending.Domain.Abstractions.Purchases;

namespace Vending.Domain.Abstractions.Snacks;

/// <summary>
///     Grain interface ISnackStatsOfPurchasesGrain
/// </summary>
public interface ISnackStatsOfPurchasesGrain : IGrainWithGuidKey
{
    /// <summary>
    ///     Asynchronously retrieves the current state of the snack.
    /// </summary>
    [AlwaysInterleave]
    Task<PurchaseStats> GetStateAsync();

    /// <summary>
    ///     Asynchronously retrieves the count of purchases that have this snack.
    /// </summary>
    /// <returns>The count of purchases that have this snack.</returns>
    [AlwaysInterleave]
    Task<int> GetCountAsync();

    /// <summary>
    ///     Asynchronously increments the count of purchases that have this snack.
    /// </summary>
    /// <param name="command">The number of purchases that have this snack </param>
    Task<Result> IncrementCountAsync(SnackIncrementBoughtCountCommand command);

    /// <summary>
    ///     Asynchronously decrements the count of purchases that have this snack.
    /// </summary>
    /// <param name="command">The number of purchases that have this snack </param>
    Task<Result> DecrementCountAsync(SnackDecrementBoughtCountCommand command);

    /// <summary>
    ///     Asynchronously retrieves the amount of purchase that have this snack.
    /// </summary>
    /// <returns>The amount of purchases that have this snack.</returns>
    [AlwaysInterleave]
    Task<decimal> GetAmountAsync();

    /// <summary>
    ///     Asynchronously increments the amount of purchases that have this snack.
    /// </summary>
    /// <param name="command">The number of purchases made for this snack</param>
    Task<Result> IncrementAmountAsync(SnackIncrementBoughtAmountCommand command);

    /// <summary>
    ///     Asynchronously decrements the amount of purchases that have this snack.
    /// </summary>
    /// <param name="command">The number of purchases made for this snack</param>
    Task<Result> DecrementAmountAsync(SnackDecrementBoughtAmountCommand command);
}
