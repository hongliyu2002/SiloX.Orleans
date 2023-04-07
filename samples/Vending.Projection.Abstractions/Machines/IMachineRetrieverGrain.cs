using System.Collections.Immutable;
using Orleans.FluentResults;

namespace Vending.Projection.Abstractions.Machines;

/// <summary>
///     The grain for machines management.
/// </summary>
public interface IMachineRetrieverGrain : IGrainWithStringKey
{
    /// <summary>
    ///     Asynchronously retrieves a list of machines.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of machines.</returns>
    Task<Result<ImmutableList<MachineInfo>>> ListAsync(MachineRetrieverListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a paged list of machines.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of machines.</returns>
    Task<Result<ImmutableList<MachineInfo>>> PagedListAsync(MachineRetrieverPagedListQuery query);
}
