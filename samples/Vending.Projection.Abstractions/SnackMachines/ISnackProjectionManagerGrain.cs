using System.Collections.Immutable;
using Orleans.Concurrency;
using Orleans.FluentResults;

namespace Vending.Projection.Abstractions.SnackMachines;

/// <summary>
///     The grain for snack machines management.
/// </summary>
public interface ISnackMachineProjectionManagerGrain : IGrainWithStringKey
{
    /// <summary>
    ///     Asynchronously retrieves a list of snack machines.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snack machines.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<SnackMachine>>> ListAsync(SnackMachineProjectionManagerListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a paged list of snack machines.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snack machines.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<SnackMachine>>> PagedListAsync(SnackMachineProjectionManagerPagedListQuery query);
}
