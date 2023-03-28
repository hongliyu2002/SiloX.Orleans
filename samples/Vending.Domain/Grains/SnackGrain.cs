﻿using Fluxera.Utilities.Extensions;
using Orleans.FluentResults;
using Orleans.Providers;
using SiloX.Domain.Abstractions;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Commands;
using Vending.Domain.Abstractions.Events;
using Vending.Domain.Abstractions.Grains;
using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Grains;

[LogConsistencyProvider(ProviderName = Constants.LogConsistencyName1)]
[StorageProvider(ProviderName = Constants.GrainStorageName1)]
public sealed class SnackGrain
    : EventSourcingGrain<Snack, SnackEvent, SnackErrorEvent>,
      ISnackGrain
{
    /// <inheritdoc />
    public SnackGrain()
        : base(Constants.StreamProviderName1)
    {
    }

    /// <inheritdoc />
    protected override string GetStreamNamespace()
    {
        return Constants.SnacksNamespace;
    }

    /// <inheritdoc />
    protected override Task<bool> PersistAsync(SnackEvent domainEvent)
    {
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public Task<Snack> GetStateAsync()
    {
        return Task.FromResult(State);
    }

    /// <inheritdoc />
    public Task<int> GetVersionAsync()
    {
        return Task.FromResult(Version);
    }

    private Result ValidateInitialize(SnackInitializeCommand command)
    {
        var id = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack {id} has already been removed.")
                     .Verify(State.IsCreated == false, $"Snack {id} already exists.")
                     .Verify(command.Name.IsNotNullOrWhiteSpace(), $"The name of snack {id} should not be empty.")
                     .Verify(command.Name.Length <= 100, $"The name of snack {id} is too long.")
                     .Verify(command.PictureUrl == null || command.PictureUrl!.Length <= 500, $"The picture url of snack {id} is too long.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace, "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanInitializeAsync(SnackInitializeCommand command)
    {
        return Task.FromResult(ValidateInitialize(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> InitializeAsync(SnackInitializeCommand command)
    {
        var id = this.GetPrimaryKey();
        return ValidateInitialize(command)
              .TapErrorTryAsync(errors => PublishOnErrorAsync(new SnackErrorEvent(id, Version, 101, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .TapTryAsync(() => PublishOnPersistedAsync(new SnackInitializedEvent(id, Version, command.Name, command.PictureUrl, command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateRemove(SnackRemoveCommand command)
    {
        var id = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack {id} has already been removed.")
                     .Verify(State.IsCreated, $"Snack {id} is not initialized.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace, "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanRemoveAsync(SnackRemoveCommand command)
    {
        return Task.FromResult(ValidateRemove(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> RemoveAsync(SnackRemoveCommand command)
    {
        var id = this.GetPrimaryKey();
        return ValidateRemove(command)
              .TapErrorTryAsync(errors => PublishOnErrorAsync(new SnackErrorEvent(id, Version, 102, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .TapTryAsync(() => PublishOnPersistedAsync(new SnackRemovedEvent(id, Version, command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateChangeName(SnackChangeNameCommand command)
    {
        var id = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack {id} has already been removed.")
                     .Verify(State.IsCreated, $"Snack {id} is not initialized.")
                     .Verify(command.Name.IsNotNullOrWhiteSpace(), $"The name of snack {id} should not be empty.")
                     .Verify(command.Name.Length <= 100, $"The name of snack {id} is too long.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace, "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanChangeNameAsync(SnackChangeNameCommand command)
    {
        return Task.FromResult(ValidateChangeName(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> ChangeNameAsync(SnackChangeNameCommand command)
    {
        var id = this.GetPrimaryKey();
        return ValidateChangeName(command)
              .TapErrorTryAsync(errors => PublishOnErrorAsync(new SnackErrorEvent(id, Version, 103, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .TapTryAsync(() => PublishOnPersistedAsync(new SnackNameChangedEvent(id, Version, command.Name, command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateChangePictureUrl(SnackChangePictureUrlCommand command)
    {
        var id = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack {id} has already been removed.")
                     .Verify(State.IsCreated, $"Snack {id} is not initialized.")
                     .Verify(command.PictureUrl.IsNullOrWhiteSpace() || command.PictureUrl!.Length <= 500, $"The picture url of snack {id} is too long.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace, "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanChangePictureUrlAsync(SnackChangePictureUrlCommand command)
    {
        return Task.FromResult(ValidateChangePictureUrl(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> ChangePictureUrlAsync(SnackChangePictureUrlCommand command)
    {
        var id = this.GetPrimaryKey();
        return ValidateChangePictureUrl(command)
              .TapErrorTryAsync(errors => PublishOnErrorAsync(new SnackErrorEvent(id, Version, 104, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .TapTryAsync(() => PublishOnPersistedAsync(new SnackPictureUrlChangedEvent(id, Version, command.PictureUrl, command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }
}
