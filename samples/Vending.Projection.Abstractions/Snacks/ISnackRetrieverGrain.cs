using System.Collections.Immutable;
using Orleans.FluentResults;

namespace Vending.Projection.Abstractions.Snacks;

/// <summary>
///     The grain for snacks management.
/// </summary>
public interface ISnackRetrieverGrain : IGrainWithStringKey
{
    /// <summary>
    ///     Asynchronously retrieves a list of snacks.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snacks.</returns>
    Task<Result<ImmutableList<SnackInfo>>> ListAsync(SnackRetrieverListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a paged list of snacks.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snacks.</returns>
    Task<Result<ImmutableList<SnackInfo>>> PagedListAsync(SnackRetrieverPagedListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a list of snacks with searching feature.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snacks.</returns>
    Task<Result<ImmutableList<SnackInfo>>> SearchingListAsync(SnackRetrieverSearchingListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a paged list of snacks with searching feature.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snacks.</returns>
    Task<Result<ImmutableList<SnackInfo>>> SearchingPagedListAsync(SnackRetrieverSearchingPagedListQuery query);
}
