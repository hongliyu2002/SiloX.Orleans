using Orleans.Concurrency;
using Orleans.FluentResults;
using SiloX.Orleans.Clustering.UnitTests.Shared.Commands;
using SiloX.Orleans.UnitTests.Shared.Commands;
using SiloX.Orleans.UnitTests.Shared.States;

namespace SiloX.Orleans.UnitTests.EventSourcing.Grains;

public interface ISnackGrain : IGrainWithGuidKey
{
    [AlwaysInterleave]
    Task<Result<Snack>> GetAsync();

    [AlwaysInterleave]
    Task<bool> CanInitializeAsync();

    Task<Result<bool>> InitializeAsync(SnackInitializeCommand cmd);

    [AlwaysInterleave]
    Task<bool> CanRemoveAsync();

    Task<Result<bool>> RemoveAsync(SnackRemoveCommand cmd);

    [AlwaysInterleave]
    Task<bool> CanChangeNameAsync();

    Task<Result<bool>> ChangeNameAsync(SnackChangeNameCommand cmd);
}
