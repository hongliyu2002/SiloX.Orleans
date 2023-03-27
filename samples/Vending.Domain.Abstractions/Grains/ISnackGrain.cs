using Orleans.Concurrency;
using Orleans.FluentResults;
using Vending.Domain.Abstractions.Commands;
using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Abstractions.Grains;

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
