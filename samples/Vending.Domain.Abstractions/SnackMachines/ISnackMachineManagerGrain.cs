using System.Collections.Immutable;
using Orleans.Concurrency;
using Orleans.FluentResults;

namespace Vending.Domain.Abstractions.SnackMachines;

/// <summary>
///     The grain for snack machines management.
/// </summary>
public interface ISnackMachineManagerGrain : IGrainWithStringKey
{
    /// <summary>
    ///     Asynchronously retrieves a list of snack machines.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snack machines.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<SnackMachine>>> ListAsync(SnackMachineManagerListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a paged list of snack machines.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snack machines.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<SnackMachine>>> PagedListAsync(SnackMachineManagerPagedListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a list of snack machines with searching feature.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snack machines.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<SnackMachine>>> SearchingListAsync(SnackMachineManagerSearchingListQuery query);

    /// <summary>
    ///     Asynchronously retrieves a paged list of snack machines with searching feature.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>List of snack machines.</returns>
    [AlwaysInterleave]
    Task<Result<ImmutableList<SnackMachine>>> SearchingPagedListAsync(SnackMachineManagerSearchingPagedListQuery query);

    /// <summary>
    ///     Asynchronously creates a new snack machine.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result<SnackMachine>> CreateAsync(SnackMachineManagerCreateCommand command);

    /// <summary>
    ///     Asynchronously deletes a snack machine.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result<Guid>> DeleteAsync(SnackMachineManagerDeleteCommand command);

    /// <summary>
    ///     Asynchronously deletes many snack machines.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result<ImmutableList<Guid>>> DeleteManyAsync(SnackMachineManagerDeleteManyCommand command);
}
