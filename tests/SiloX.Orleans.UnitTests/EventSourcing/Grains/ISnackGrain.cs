using Orleans.Concurrency;
using Orleans.FluentResults;
using SiloX.Orleans.UnitTests.Shared.Commands;
using SiloX.Orleans.UnitTests.Shared.States;

namespace SiloX.Orleans.UnitTests.EventSourcing.Grains;

public interface ISnackGrain : IGrainWithGuidKey
{
    [AlwaysInterleave]
    Task<Result<Snack>> GetAsync();

    [AlwaysInterleave]
    Task<bool> CanInitializeAsync(SnackInitializeCommand command);

    Task<Result> InitializeAsync(SnackInitializeCommand command);

    [AlwaysInterleave]
    Task<bool> CanRemoveAsync(SnackRemoveCommand command);

    Task<Result> RemoveAsync(SnackRemoveCommand command);

    [AlwaysInterleave]
    Task<bool> CanChangeNameAsync(SnackChangeNameCommand command);

    Task<Result> ChangeNameAsync(SnackChangeNameCommand command);

    [AlwaysInterleave]
    Task<bool> CanChangePictureUrlAsync(SnackChangePictureUrlCommand command);

    Task<Result> ChangePictureUrlAsync(SnackChangePictureUrlCommand command);
}
