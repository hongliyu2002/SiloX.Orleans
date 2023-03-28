using System.Collections.Immutable;
using Orleans.Concurrency;
using Orleans.FluentResults;
using Vending.Domain.Abstractions.Commands;
using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Abstractions.Grains;

/// <summary>
///     This interface defines the contract for the SnackMachineGrain
/// </summary>
public interface ISnackMachineGrain : IGrainWithGuidKey
{
    /// <summary>
    ///     Asynchronously retrieves the current state of the SnackMachine
    /// </summary>
    [AlwaysInterleave]
    Task<SnackMachine> GetStateAsync();

    /// <summary>
    ///     Asynchronously retrieves the current version number of the SnackMachine
    /// </summary>
    [AlwaysInterleave]
    Task<int> GetVersionAsync();

    /// <summary>
    ///     Asynchronously retrieves the current amount of money inside the SnackMachine
    /// </summary>
    [AlwaysInterleave]
    Task<Result<Money>> GetMoneyInsideAsync();

    /// <summary>
    ///     Asynchronously retrieves the current amount of money in transaction
    /// </summary>
    [AlwaysInterleave]
    Task<Result<decimal>> GetAmountInTransactionAsync();

    /// <summary>
    ///     Asynchronously retrieves the current list of slots in the SnackMachine
    /// </summary>
    [AlwaysInterleave]
    Task<Result<ImmutableList<Slot>>> GetSlotsAsync();

    /// <summary>
    ///     Asynchronously checks whether the SnackMachine can be initialized with the given command
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanInitializeAsync(SnackMachineInitializeCommand command);

    /// <summary>
    ///     Asynchronously initializes the SnackMachine with the given command
    /// </summary>
    Task<Result> InitializeAsync(SnackMachineInitializeCommand command);

    /// <summary>
    ///     Asynchronously checks whether the SnackMachine can be removed with the given command
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanRemoveAsync(SnackMachineRemoveCommand command);

    /// <summary>
    ///     Asynchronously removes the SnackMachine with the given command
    /// </summary>
    Task<Result> RemoveAsync(SnackMachineRemoveCommand command);

    /// <summary>
    ///     Asynchronously checks whether the SnackMachine can have money loaded with the given command
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanLoadMoneyAsync(SnackMachineLoadMoneyCommand command);

    /// <summary>
    ///     Asynchronously loads money into the SnackMachine with the given command
    /// </summary>
    Task<Result> LoadMoneyAsync(SnackMachineLoadMoneyCommand command);

    /// <summary>
    ///     Asynchronously checks whether the SnackMachine can have money unloaded with the given command
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanUnloadMoneyAsync(SnackMachineUnloadMoneyCommand command);

    /// <summary>
    ///     Asynchronously unloads money from the SnackMachine with the given command
    /// </summary>
    Task<Result> UnloadMoneyAsync(SnackMachineUnloadMoneyCommand command);

    /// <summary>
    ///     Asynchronously checks whether the SnackMachine can have money inserted with the given command
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanInsertMoneyAsync(SnackMachineInsertMoneyCommand command);

    /// <summary>
    ///     Asynchronously inserts money into the SnackMachine with the given command
    /// </summary>
    Task<Result> InsertMoneyAsync(SnackMachineInsertMoneyCommand command);

    /// <summary>
    ///     Asynchronously checks whether the SnackMachine can return money with the given command
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanReturnMoneyAsync(SnackMachineReturnMoneyCommand command);

    /// <summary>
    ///     Asynchronously returns money from the SnackMachine with the given command
    /// </summary>
    Task<Result> ReturnMoneyAsync(SnackMachineReturnMoneyCommand command);

    /// <summary>
    ///     Asynchronously checks whether the SnackMachine can have snacks loaded with the given command
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanLoadSnacksAsync(SnackMachineLoadSnacksCommand command);

    /// <summary>
    ///     Asynchronously loads snacks into the SnackMachine with the given command
    /// </summary>
    Task<Result> LoadSnacksAsync(SnackMachineLoadSnacksCommand command);

    /// <summary>
    ///     Asynchronously checks whether the SnackMachine can have a snack bought with the given command
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanBuySnackAsync(SnackMachineBuySnackCommand command);

    /// <summary>
    ///     Asynchronously buys a snack from the SnackMachine with the given command
    /// </summary>
    Task<Result> BuySnackAsync(SnackMachineBuySnackCommand command);
}
