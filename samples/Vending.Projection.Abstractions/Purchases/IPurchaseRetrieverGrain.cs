using System.Collections.Immutable;
using Orleans.FluentResults;

namespace Vending.Projection.Abstractions.Purchases;

/// <summary>
///     The grain for purchases management.
/// </summary>
public interface IPurchaseRetrieverGrain : IGrainWithStringKey
{
    /// <summary>
    ///     Asynchronously retrieves a list of purchases.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of purchases.</returns>
    Task<Result<ImmutableList<Purchase>>> ListAsync(PurchaseRetrieverListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a paged list of purchases.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of purchases.</returns>
    Task<Result<ImmutableList<Purchase>>> PagedListAsync(PurchaseRetrieverPagedListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a list of purchases with searching feature.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of purchases.</returns>
    Task<Result<ImmutableList<Purchase>>> SearchingListAsync(PurchaseRetrieverSearchingListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a paged list of purchases with searching feature.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of purchases.</returns>
    Task<Result<ImmutableList<Purchase>>> SearchingPagedListAsync(PurchaseRetrieverSearchingPagedListQuery query);
}
