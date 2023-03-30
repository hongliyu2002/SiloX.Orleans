using Orleans.Concurrency;
using Orleans.FluentResults;
using Vending.Domain.Abstractions.Commands;
using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Abstractions.Grains;

/// <summary>
///     This interface defines the contract for the SnackGrain
/// </summary>
public interface ISnackGrain
{
    /// <summary>
    ///     Asynchronously retrieves the current state of the Snack
    /// </summary>
    [AlwaysInterleave]
    Task<Snack> GetStateAsync();

    /// <summary>
    ///     Asynchronously retrieves the current version number of the Snack
    /// </summary>
    [AlwaysInterleave]
    Task<int> GetVersionAsync();

    /// <summary>
    ///     Asynchronously checks whether the Snack can be initialized with the given command
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanInitializeAsync(SnackInitializeCommand command);

    /// <summary>
    ///     Asynchronously initializes the Snack with the given command
    /// </summary>
    Task<Result> InitializeAsync(SnackInitializeCommand command);

    /// <summary>
    ///     Asynchronously checks whether the Snack can be removed with the given command
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanRemoveAsync(SnackRemoveCommand command);

    /// <summary>
    ///     Asynchronously removes the Snack with the given command
    /// </summary>
    Task<Result> RemoveAsync(SnackRemoveCommand command);

    /// <summary>
    ///     Asynchronously checks whether the Snack's name can be changed with the given command
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanChangeNameAsync(SnackChangeNameCommand command);

    /// <summary>
    ///     Asynchronously changes the Snack's name with the given command
    /// </summary>
    Task<Result> ChangeNameAsync(SnackChangeNameCommand command);

    /// <summary>
    ///     Asynchronously checks whether the Snack's picture URL can be changed with the given command
    /// </summary>
    [AlwaysInterleave]
    Task<bool> CanChangePictureUrlAsync(SnackChangePictureUrlCommand command);

    /// <summary>
    ///     Asynchronously changes the Snack's picture URL with the given command
    /// </summary>
    Task<Result> ChangePictureUrlAsync(SnackChangePictureUrlCommand command);
}
