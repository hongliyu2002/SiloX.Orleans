using System.Collections.Immutable;
using Orleans.FluentResults;

namespace Vending.Projection.Abstractions.Machines;

/// <summary>
///     The grain for snack machines management.
/// </summary>
public interface IMachineRetrieverGrain : IGrainWithStringKey
{
    /// <summary>
    ///     Asynchronously retrieves a list of snack machines.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snack machines.</returns>
    Task<Result<ImmutableList<MachineInfo>>> ListAsync(MachineRetrieverListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a paged list of snack machines.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snack machines.</returns>
    Task<Result<ImmutableList<MachineInfo>>> PagedListAsync(MachineRetrieverPagedListQuery query);
}
