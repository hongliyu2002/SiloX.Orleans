using System.Collections.Immutable;
using Orleans.Concurrency;
using Orleans.FluentResults;

namespace Vending.Domain.Abstractions.Snacks;

/// <summary>
///     The grain for snacks management.
/// </summary>
public interface ISnackManagerGrain : IGrainWithStringKey
{
    /// <summary>
    ///     Asynchronously retrieves a list of snacks.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snacks.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<Snack>>> ListAsync(SnackManagerListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a paged list of snacks.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snacks.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<Snack>>> PagedListAsync(SnackManagerPagedListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a list of snacks with searching feature.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snacks.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<Snack>>> SearchingListAsync(SnackManagerSearchingListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a paged list of snacks with searching feature.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snacks.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<Snack>>> SearchingPagedListAsync(SnackManagerSearchingPagedListQuery query);

    /// <summary>
    ///     Asynchronously creates a new snack.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result<Guid>> CreateAsync(SnackManagerCreateCommand command);

    /// <summary>
    ///     Asynchronously deletes a snack.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result> DeleteAsync(SnackManagerDeleteCommand command);

    /// <summary>
    ///     Asynchronously deletes many snacks.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result> DeleteManyAsync(SnackManagerDeleteManyCommand command);
}
