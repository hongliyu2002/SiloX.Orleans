using System.Collections.Immutable;
using Orleans.Concurrency;
using Orleans.FluentResults;
using Vending.Domain.Abstractions.Commands;
using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Abstractions.Grains;

public interface ISnackMachineGrain : IGrainWithGuidKey
{
    [AlwaysInterleave]
    Task<Result<SnackMachine>> GetAsync();

    [AlwaysInterleave]
    Task<Result<Money>> GetMoneyInsideAsync();

    [AlwaysInterleave]
    Task<Result<decimal>> GetAmountInTransactionAsync();

    [AlwaysInterleave]
    Task<Result<ImmutableList<Slot>>> GetSlotsAsync();

    [AlwaysInterleave]
    Task<bool> CanInitializeAsync(SnackMachineInitializeCommand command);

    Task<Result> InitializeAsync(SnackMachineInitializeCommand command);

    [AlwaysInterleave]
    Task<bool> CanRemoveAsync(SnackMachineRemoveCommand command);

    Task<Result> RemoveAsync(SnackMachineRemoveCommand command);

    [AlwaysInterleave]
    Task<bool> CanLoadMoneyAsync(SnackMachineLoadMoneyCommand command);

    Task<Result> LoadMoneyAsync(SnackMachineLoadMoneyCommand command);

    [AlwaysInterleave]
    Task<bool> CanUnloadMoneyAsync(SnackMachineUnloadMoneyCommand command);

    Task<Result> UnloadMoneyAsync(SnackMachineUnloadMoneyCommand command);

    [AlwaysInterleave]
    Task<bool> CanInsertMoneyAsync(SnackMachineInsertMoneyCommand command);

    Task<Result> InsertMoneyAsync(SnackMachineInsertMoneyCommand command);

    [AlwaysInterleave]
    Task<bool> CanReturnMoneyAsync(SnackMachineReturnMoneyCommand command);

    Task<Result> ReturnMoneyAsync(SnackMachineReturnMoneyCommand command);

    [AlwaysInterleave]
    Task<bool> CanLoadSnacksAsync(SnackMachineLoadSnacksCommand command);

    Task<Result> LoadSnacksAsync(SnackMachineLoadSnacksCommand command);

    [AlwaysInterleave]
    Task<bool> CanBuySnackAsync(SnackMachineBuySnackCommand command);

    Task<Result> BuySnackAsync(SnackMachineBuySnackCommand command);
}
