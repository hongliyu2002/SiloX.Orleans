using Orleans.FluentResults;

namespace Vending.Domain.Abstractions.Machines;

/// <summary>
///     The grain for snack machines management.
/// </summary>
public interface IMachineRepoGrain : IGrainWithStringKey
{
    /// <summary>
    ///     Asynchronously creates a new snack machine.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result<Guid>> CreateAsync(MachineRepoCreateCommand command);

    /// <summary>
    ///     Asynchronously deletes a snack machine.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result> DeleteAsync(MachineRepoDeleteCommand command);

    /// <summary>
    ///     Asynchronously deletes many snack machines.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result> DeleteManyAsync(MachineRepoDeleteManyCommand command);
}
