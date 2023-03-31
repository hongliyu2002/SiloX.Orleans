﻿using Orleans.Concurrency;
using Orleans.FluentResults;
using Vending.Domain.Abstractions.Commands;
using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Abstractions.Grains;

/// <summary>
///     Grain interface ISnackSnackMachineStatsGrain
/// </summary>
public interface ISnackSnackMachineStatsGrain : IGrainWithGuidKey
{
    /// <summary>
    ///     Asynchronously retrieves the current state of the Snack
    /// </summary>
    [AlwaysInterleave]
    Task<SnackMachineStats> GetStateAsync();

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