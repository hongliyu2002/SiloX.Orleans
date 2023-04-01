using System.Collections.Immutable;
using Orleans.Concurrency;
using Orleans.FluentResults;

namespace Vending.Projection.Abstractions.Purchases;

/// <summary>
///     The grain for purchases management.
/// </summary>
public interface IPurchaseProjectionManagerGrain : IGrainWithStringKey
{
    /// <summary>
    ///     Asynchronously retrieves a list of purchases.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of purchases.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<Purchase>>> ListAsync(PurchaseProjectionManagerListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a paged list of purchases.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of purchases.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<Purchase>>> PagedListAsync(PurchaseProjectionManagerPagedListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a list of purchases with searching feature.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of purchases.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<Purchase>>> SearchingListAsync(PurchaseProjectionManagerSearchingListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a paged list of purchases with searching feature.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of purchases.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<Purchase>>> SearchingPagedListAsync(PurchaseProjectionManagerSearchingPagedListQuery query);
}
