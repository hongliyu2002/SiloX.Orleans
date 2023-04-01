using System.Collections.Immutable;
using Orleans.Concurrency;
using Orleans.FluentResults;

namespace Vending.Domain.Abstractions.Purchases;

/// <summary>
///     The grain for purchases management.
/// </summary>
public interface IPurchaseRepoGrain : IGrainWithStringKey
{
    /// <summary>
    ///     Asynchronously creates a new purchase.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result<Guid>> CreateAsync(PurchaseRepoCreateCommand command);
}
