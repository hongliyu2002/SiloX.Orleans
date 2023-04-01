using System.Collections.Immutable;
using Orleans.Concurrency;
using Orleans.FluentResults;

namespace Vending.Projection.Abstractions.Snacks;

/// <summary>
///     The grain for snacks management.
/// </summary>
public interface ISnackProjectionManagerGrain : IGrainWithStringKey
{
    /// <summary>
    ///     Asynchronously retrieves a list of snacks.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snacks.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<Snack>>> ListAsync(SnackProjectionManagerListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a paged list of snacks.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snacks.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<Snack>>> PagedListAsync(SnackProjectionManagerPagedListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a list of snacks with searching feature.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snacks.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<Snack>>> SearchingListAsync(SnackProjectionManagerSearchingListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a paged list of snacks with searching feature.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snacks.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<Snack>>> SearchingPagedListAsync(SnackProjectionManagerSearchingPagedListQuery query);
}
