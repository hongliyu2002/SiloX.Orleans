using Orleans.Concurrency;
using Orleans.FluentResults;
using Vending.Domain.Abstractions.Machines;

namespace Vending.Domain.Abstractions.Snacks;

/// <summary>
///     Grain interface ISnackStatsOfMachineGrain
/// </summary>
public interface ISnackStatsOfMachineGrain : IGrainWithGuidKey
{
    /// <summary>
    ///     Asynchronously retrieves the current state of the Snack
    /// </summary>
    [AlwaysInterleave]
    Task<MachineStat> GetStateAsync();

    /// <summary>
    ///     Asynchronously retrieves the count of machines that have this snack.
    /// </summary>
    /// <returns>The count of machines that have this snack.</returns>
    [AlwaysInterleave]
    Task<int> GetCountAsync();

    /// <summary>
    ///     Asynchronously increments the count of machines that have this snack
    /// </summary>
    /// <param name="command">The number of machines that have this snack </param>
    Task<Result> IncrementCountAsync(SnackIncrementMachineCountCommand command);

    /// <summary>
    ///     Asynchronously decrements the count of machines that have this snack
    /// </summary>
    /// <param name="command">The number of machines that have this snack </param>
    Task<Result> DecrementCountAsync(SnackDecrementMachineCountCommand command);
}
