using Orleans.Concurrency;
using Orleans.FluentResults;
using Vending.Domain.Abstractions.Machines;

namespace Vending.Domain.Abstractions.Snacks;

/// <summary>
///     Grain interface ISnackStatsOfMachinesGrain
/// </summary>
public interface ISnackStatsOfMachinesGrain : IGrainWithGuidKey
{
    /// <summary>
    ///     Asynchronously retrieves the current state of the Snack
    /// </summary>
    [AlwaysInterleave]
    Task<StatsOfMachines> GetStatsOfMachinesAsync();

    /// <summary>
    ///     Asynchronously retrieves the count of machines that have this snack.
    /// </summary>
    /// <returns>The count of machines that have this snack.</returns>
    [AlwaysInterleave]
    Task<int> GetMachineCountAsync();

    /// <summary>
    ///     Asynchronously retrieves the quantity in machines that have this snack.
    /// </summary>
    /// <returns>The quantity in machines that have this snack.</returns>
    [AlwaysInterleave]
    Task<int> GetTotalQuantityAsync();

    /// <summary>
    ///     Asynchronously retrieves the amount in machines that have this snack.
    /// </summary>
    /// <returns>The amount in machines that have this snack.</returns>
    [AlwaysInterleave]
    Task<decimal> GetTotalAmountAsync();

    /// <summary>
    ///     Asynchronously update the count of machines that have this snack.
    /// </summary>
    /// <param name="machineCount">The count of machines that have this snack.</param>
    Task<Result> UpdateMachineCountAsync(int machineCount);

    /// <summary>
    ///     Asynchronously update the quantity in machines that have this snack.
    /// </summary>
    /// <param name="totalQuantity">The quantity in machines that have this snack.</param>
    Task<Result> UpdateTotalQuantityAsync(int totalQuantity);

    /// <summary>
    ///     Asynchronously update the amount in machines that have this snack.
    /// </summary>
    /// <param name="totalAmount">The amount in machines that have this snack.</param>
    Task<Result> UpdateTotalAmountAsync(decimal totalAmount);
}
