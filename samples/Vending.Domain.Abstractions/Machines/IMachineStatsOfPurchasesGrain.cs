using Orleans.Concurrency;
using Orleans.FluentResults;
using Vending.Domain.Abstractions.Purchases;

namespace Vending.Domain.Abstractions.Machines;

/// <summary>
///     Grain interface IMachineStatsOfPurchasesGrain
/// </summary>
public interface IMachineStatsOfPurchasesGrain : IGrainWithGuidKey
{
    /// <summary>
    ///     Asynchronously retrieves the current state of the machine.
    /// </summary>
    [AlwaysInterleave]
    Task<StatsOfPurchases> GetStatsOfPurchasesAsync();

    /// <summary>
    ///     Asynchronously retrieves the count of purchases that made for this machine.
    /// </summary>
    /// <returns>The count of purchases that made for this machine.</returns>
    [AlwaysInterleave]
    Task<int> GetBoughtCountAsync();

    /// <summary>
    ///     Asynchronously retrieves the amount of purchases that made for this machine.
    /// </summary>
    /// <returns>The amount of purchases that made for this machine.</returns>
    [AlwaysInterleave]
    Task<decimal> GetBoughtAmountAsync();

    /// <summary>
    ///     Asynchronously updates the count of purchases that made for this machine.
    /// </summary>
    /// <param name="boughtCount">The count of purchases that made for this machine.</param>
    Task<Result> UpdateBoughtCountAsync(int boughtCount);

    /// <summary>
    ///     Asynchronously updates the amount of purchases that made for this machine.
    /// </summary>
    /// <param name="boughtAmount">The amount of purchases that made for this machine.</param>
    Task<Result> UpdateBoughtAmountAsync(decimal boughtAmount);
}
