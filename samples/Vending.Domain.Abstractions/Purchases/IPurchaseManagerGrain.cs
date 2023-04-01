using System.Collections.Immutable;
using Orleans.Concurrency;
using Orleans.FluentResults;

namespace Vending.Domain.Abstractions.Purchases;

/// <summary>
///     The grain for purchases management.
/// </summary>
public interface IPurchaseManagerGrain : IGrainWithStringKey
{
    /// <summary>
    ///     Asynchronously retrieves a list of purchases.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of purchases.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<Purchase>>> ListAsync(PurchaseManagerListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a paged list of purchases.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of purchases.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<Purchase>>> PagedListAsync(PurchaseManagerPagedListQuery query);

    /// <summary>
    ///     Asynchronously creates a new purchase.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result<Guid>> CreateAsync(PurchaseManagerCreateCommand command);

    /// <summary>
    ///     Asynchronously deletes a purchase.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result> DeleteAsync(PurchaseManagerDeleteCommand command);

    /// <summary>
    ///     Asynchronously deletes many purchases.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result> DeleteManyAsync(PurchaseManagerDeleteManyCommand command);
}
