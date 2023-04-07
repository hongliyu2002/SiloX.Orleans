using System.Collections.Immutable;
using Orleans.Concurrency;
using Orleans.FluentResults;

namespace Vending.Domain.Abstractions.Machines;

/// <summary>
///     This interface defines the contract for the MachineGrain
/// </summary>
public interface IMachineGrain : IGrainWithGuidKey
{
    /// <summary>
    ///     Asynchronously retrieves the current state of the machine.
    /// </summary>
    [AlwaysInterleave]
    Task<Machine> GetStateAsync();

    /// <summary>
    ///     Asynchronously retrieves the current version number of the machine.
    /// </summary>
    [AlwaysInterleave]
    Task<int> GetVersionAsync();

    /// <summary>
    ///     Asynchronously retrieves the current amount of money inside the Machine.
    /// </summary>
    [AlwaysInterleave]
    Task<Money> GetMoneyInsideAsync();

    /// <summary>
    ///     Asynchronously retrieves the current amount of money in transaction.
    /// </summary>
    [AlwaysInterleave]
    Task<decimal> GetAmountInTransactionAsync();

    /// <summary>
    ///     Asynchronously retrieves the current list of slots in the Machine.
    /// </summary>
    [AlwaysInterleave]
    Task<ImmutableList<Slot>> GetSlotsAsync();

    /// <summary>
    ///     Asynchronously checks whether the Machine can be initialized with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanInitializeAsync(MachineInitializeCommand command);

    /// <summary>
    ///     Asynchronously initializes the Machine with the given command.
    /// </summary>
    Task<Result> InitializeAsync(MachineInitializeCommand command);

    /// <summary>
    ///     Asynchronously checks whether the Machine can be removed with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanRemoveAsync(MachineRemoveCommand command);

    /// <summary>
    ///     Asynchronously removes the Machine with the given command.
    /// </summary>
    Task<Result> RemoveAsync(MachineRemoveCommand command);

    /// <summary>
    ///     Asynchronously checks whether the Machine can have money loaded with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanLoadMoneyAsync(MachineLoadMoneyCommand command);

    /// <summary>
    ///     Asynchronously loads money into the Machine with the given command.
    /// </summary>
    Task<Result> LoadMoneyAsync(MachineLoadMoneyCommand command);

    /// <summary>
    ///     Asynchronously checks whether the Machine can have money unloaded with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanUnloadMoneyAsync(MachineUnloadMoneyCommand command);

    /// <summary>
    ///     Asynchronously unloads money from the Machine with the given command.
    /// </summary>
    Task<Result> UnloadMoneyAsync(MachineUnloadMoneyCommand command);

    /// <summary>
    ///     Asynchronously checks whether the Machine can have money inserted with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanInsertMoneyAsync(MachineInsertMoneyCommand command);

    /// <summary>
    ///     Asynchronously inserts money into the Machine with the given command.
    /// </summary>
    Task<Result> InsertMoneyAsync(MachineInsertMoneyCommand command);

    /// <summary>
    ///     Asynchronously checks whether the Machine can return money with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanReturnMoneyAsync(MachineReturnMoneyCommand command);

    /// <summary>
    ///     Asynchronously returns money from the Machine with the given command.
    /// </summary>
    Task<Result> ReturnMoneyAsync(MachineReturnMoneyCommand command);

    /// <summary>
    ///     Asynchronously checks whether the Machine can have snacks loaded with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanLoadSnacksAsync(MachineLoadCommand command);

    /// <summary>
    ///     Asynchronously loads snacks into the Machine with the given command.
    /// </summary>
    Task<Result> LoadSnacksAsync(MachineLoadCommand command);

    /// <summary>
    ///     Asynchronously checks whether the Machine can have snacks unloaded with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanUnloadSnacksAsync(MachineUnloadCommand command);

    /// <summary>
    ///     Asynchronously unloads snacks from the Machine with the given command.
    /// </summary>
    Task<Result> UnloadSnacksAsync(MachineUnloadCommand command);

    /// <summary>
    ///     Asynchronously checks whether the Machine can have a snack bought with the given command.
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanBuySnackAsync(MachineBuyCommand command);

    /// <summary>
    ///     Asynchronously buys a snack from the Machine with the given command.
    /// </summary>
    Task<Result> BuySnackAsync(MachineBuyCommand command);
}
