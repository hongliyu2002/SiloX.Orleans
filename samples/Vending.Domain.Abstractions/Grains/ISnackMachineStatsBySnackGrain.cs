using Orleans.Concurrency;
using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Abstractions.Grains;

/// <summary>
///    Grain interface ISnackMachineStatsBySnackGrain
/// </summary>
public interface ISnackMachineStatsBySnackGrain : IGrainWithGuidKey
{
    /// <summary>
    ///     Asynchronously retrieves the current state of the Snack
    /// </summary>
    [AlwaysInterleave]
    Task<SnackMachineStatsBySnack> GetStateAsync();

    /// <summary>
    ///     Asynchronously retrieves the count of machines that have this snack.
    /// </summary>
    /// <returns>The count of machines that have this snack.</returns>
    Task<int> GetMachineCountAsync();

    /// <summary>
    ///     Asynchronously increments the count of machines that have this snack
    /// </summary>
    /// <param name="numOfMachines">The number of machines that have this snack </param>
    Task IncrementCountAsync(int numOfMachines);

    /// <summary>
    ///     Asynchronously decrements the count of machines that have this snack
    /// </summary>
    /// <param name="numOfMachines">The number of machines that have this snack </param>
    Task DecrementCountAsync(int numOfMachines);
}
