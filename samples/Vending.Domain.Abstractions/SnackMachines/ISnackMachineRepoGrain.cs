using Orleans.FluentResults;

namespace Vending.Domain.Abstractions.SnackMachines;

/// <summary>
///     The grain for snack machines management.
/// </summary>
public interface ISnackMachineRepoGrain : IGrainWithStringKey
{
    /// <summary>
    ///     Asynchronously creates a new snack machine.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result<Guid>> CreateAsync(SnackMachineRepoCreateCommand command);

    /// <summary>
    ///     Asynchronously deletes a snack machine.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result> DeleteAsync(SnackMachineRepoDeleteCommand command);

    /// <summary>
    ///     Asynchronously deletes many snack machines.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result> DeleteManyAsync(SnackMachineRepoDeleteManyCommand command);
}
