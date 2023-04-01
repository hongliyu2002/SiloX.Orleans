using System.Collections.Immutable;
using Orleans.FluentResults;

namespace Vending.Projection.Abstractions.SnackMachines;

/// <summary>
///     The grain for snack machines management.
/// </summary>
public interface ISnackMachineRetrieverGrain : IGrainWithStringKey
{
    /// <summary>
    ///     Asynchronously retrieves a list of snack machines.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snack machines.</returns>
    Task<Result<ImmutableList<SnackMachine>>> ListAsync(SnackMachineRetrieverListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a paged list of snack machines.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snack machines.</returns>
    Task<Result<ImmutableList<SnackMachine>>> PagedListAsync(SnackMachineRetrieverPagedListQuery query);
}
