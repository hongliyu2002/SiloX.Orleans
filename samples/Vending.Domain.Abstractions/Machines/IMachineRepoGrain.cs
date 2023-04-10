using Orleans.FluentResults;

namespace Vending.Domain.Abstractions.Machines;

/// <summary>
///     The grain for machines management.
/// </summary>
public interface IMachineRepoGrain : IGrainWithStringKey
{
    /// <summary>
    ///     Asynchronously creates a new machine.
    /// </summary>
    /// <param name="command"></param>
    Task<Result<Machine>> CreateAsync(MachineRepoCreateCommand command);

    /// <summary>
    ///     Asynchronously updates a machine.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<Result<Machine>> UpdateAsync(MachineRepoUpdateCommand command);

    /// <summary>
    ///     Asynchronously deletes a machine.
    /// </summary>
    /// <param name="command"></param>
    Task<Result> DeleteAsync(MachineRepoDeleteCommand command);

    /// <summary>
    ///     Asynchronously deletes many machines.
    /// </summary>
    /// <param name="command"></param>
    Task<Result> DeleteManyAsync(MachineRepoDeleteManyCommand command);
}