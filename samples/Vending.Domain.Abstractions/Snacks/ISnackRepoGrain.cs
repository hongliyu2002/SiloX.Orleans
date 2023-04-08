using Orleans.FluentResults;

namespace Vending.Domain.Abstractions.Snacks;

/// <summary>
///     The grain for snacks management.
/// </summary>
public interface ISnackRepoGrain : IGrainWithStringKey
{
    /// <summary>
    ///     Asynchronously creates a new snack.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result<Snack>> CreateAsync(SnackRepoCreateCommand command);

    /// <summary>
    ///     Asynchronously updates a snack.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result<Snack>> UpdateAsync(SnackRepoUpdateCommand command);

    /// <summary>
    ///     Asynchronously deletes a snack.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result> DeleteAsync(SnackRepoDeleteCommand command);

    /// <summary>
    ///     Asynchronously deletes many snacks.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result> DeleteManyAsync(SnackRepoDeleteManyCommand command);
}
