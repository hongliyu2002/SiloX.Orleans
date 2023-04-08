using Orleans.Concurrency;
using Orleans.FluentResults;

namespace Vending.Domain.Abstractions.Machines;

/// <summary>
///     This interface defines the contract for the machineGrain
/// </summary>
public interface IMachineGrain : IGrainWithGuidKey
{
    /// <summary>
    ///     Asynchronously retrieves the current state of the machine.
    /// </summary>
    [AlwaysInterleave]
    Task<Machine> GetMachineAsync();

    /// <summary>
    ///     Asynchronously retrieves the current version number of the machine.
    /// </summary>
    [AlwaysInterleave]
    Task<int> GetVersionAsync();

    /// <summary>
    ///     Asynchronously checks whether the machine can be initialized with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanInitializeAsync(MachineInitializeCommand command);

    /// <summary>
    ///     Asynchronously initializes the machine with the given command.
    /// </summary>
    Task<Result> InitializeAsync(MachineInitializeCommand command);

    /// <summary>
    ///     Asynchronously checks whether the machine can be removed with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanDeleteAsync(MachineDeleteCommand command);

    /// <summary>
    ///     Asynchronously removes the machine with the given command.
    /// </summary>
    Task<Result> DeleteAsync(MachineDeleteCommand command);

    /// <summary>
    ///     Asynchronously checks whether the machine can have slot added with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanAddSlotAsync(MachineAddSlotCommand command);

    /// <summary>
    ///     Asynchronously adds slot into the machine with the given command.
    /// </summary>
    Task<Result> AddSlotAsync(MachineAddSlotCommand command);

    /// <summary>
    ///     Asynchronously checks whether the machine can have slot removeed with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanRemoveSlotAsync(MachineRemoveSlotCommand command);

    /// <summary>
    ///     Asynchronously removes slot from the machine with the given command.
    /// </summary>
    Task<Result> RemoveSlotAsync(MachineRemoveSlotCommand command);

    /// <summary>
    ///     Asynchronously checks whether the machine can have money loaded with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanLoadMoneyAsync(MachineLoadMoneyCommand command);

    /// <summary>
    ///     Asynchronously loads money into the machine with the given command.
    /// </summary>
    Task<Result> LoadMoneyAsync(MachineLoadMoneyCommand command);

    /// <summary>
    ///     Asynchronously checks whether the machine can have money unloaded with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanUnloadMoneyAsync(MachineUnloadMoneyCommand command);

    /// <summary>
    ///     Asynchronously unloads money from the machine with the given command.
    /// </summary>
    Task<Result> UnloadMoneyAsync(MachineUnloadMoneyCommand command);

    /// <summary>
    ///     Asynchronously checks whether the machine can have money inserted with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanInsertMoneyAsync(MachineInsertMoneyCommand command);

    /// <summary>
    ///     Asynchronously inserts money into the machine with the given command.
    /// </summary>
    Task<Result> InsertMoneyAsync(MachineInsertMoneyCommand command);

    /// <summary>
    ///     Asynchronously checks whether the machine can return money with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanReturnMoneyAsync(MachineReturnMoneyCommand command);

    /// <summary>
    ///     Asynchronously returns money from the machine with the given command.
    /// </summary>
    Task<Result> ReturnMoneyAsync(MachineReturnMoneyCommand command);

    /// <summary>
    ///     Asynchronously checks whether the machine can have snacks loaded with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanLoadSnacksAsync(MachineLoadSnacksCommand command);

    /// <summary>
    ///     Asynchronously loads snacks into the machine with the given command.
    /// </summary>
    Task<Result> LoadSnacksAsync(MachineLoadSnacksCommand command);

    /// <summary>
    ///     Asynchronously checks whether the machine can have snacks unloaded with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanUnloadSnacksAsync(MachineUnloadSnacksCommand command);

    /// <summary>
    ///     Asynchronously unloads snacks from the machine with the given command.
    /// </summary>
    Task<Result> UnloadSnacksAsync(MachineUnloadSnacksCommand command);

    /// <summary>
    ///     Asynchronously checks whether the machine can have a snack bought with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanBuySnackAsync(MachineBuySnackCommand command);

    /// <summary>
    ///     Asynchronously buys a snack from the machine with the given command.
    /// </summary>
    Task<Result> BuySnackAsync(MachineBuySnackCommand command);
}
