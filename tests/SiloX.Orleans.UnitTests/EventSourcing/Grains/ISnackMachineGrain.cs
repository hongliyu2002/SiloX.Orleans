using System.Collections.Immutable;
using Orleans.Concurrency;
using Orleans.FluentResults;
using SiloX.Orleans.UnitTests.Shared.Commands;
using SiloX.Orleans.UnitTests.Shared.States;

namespace SiloX.Orleans.UnitTests.EventSourcing.Grains;

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
