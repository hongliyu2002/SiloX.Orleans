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
    Task<StatsOfPurchases> GetStatsOfPurchasesAsync();

    /// <summary>
    ///     Asynchronously retrieves the count of purchases that have this snack.
    /// </summary>
    /// <returns>The count of purchases that made for this snack.</returns>
    [AlwaysInterleave]
    Task<int> GetBoughtCountAsync();

    /// <summary>
    ///     Asynchronously retrieves the amount of purchases that made for this snack.
    /// </summary>
    /// <returns>The amount of purchases that made for this snack.</returns>
    [AlwaysInterleave]
    Task<decimal> GetBoughtAmountAsync();

    /// <summary>
    ///     Asynchronously updates the count of purchases that have this snack.
    /// </summary>
    /// <param name="boughtCount">The count of purchases that made for this snack.</param>
    Task<Result> UpdateBoughtCountAsync(int boughtCount);

    /// <summary>
    ///     Asynchronously updates the amount of purchases that made for this snack.
    /// </summary>
    /// <param name="boughtAmount">The amount of purchases that made for this snack.</param>
    Task<Result> UpdateBoughtAmountAsync(decimal boughtAmount);
}
