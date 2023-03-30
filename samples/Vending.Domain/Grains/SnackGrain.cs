using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using Orleans.FluentResults;
using Orleans.Providers;
using SiloX.Domain.Abstractions;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Commands;
using Vending.Domain.Abstractions.Events;
using Vending.Domain.Abstractions.Grains;
using Vending.Domain.Abstractions.States;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain.Grains;

[LogConsistencyProvider(ProviderName = Constants.LogConsistencyName)]
[StorageProvider(ProviderName = Constants.GrainStorageName)]
public sealed class SnackGrain : EventSourcingGrainWithGuidKey<Snack, SnackCommand, SnackEvent, SnackErrorEvent>, ISnackGrain
{
    private readonly DomainDbContext _dbContext;

    /// <inheritdoc />
    public SnackGrain(DomainDbContext dbContext) : base(Constants.StreamProviderName)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
    }

    /// <inheritdoc />
    protected override string GetStreamNamespace()
    {
        return Constants.SnacksNamespace;
    }

    /// <inheritdoc />
    protected override string GetBroadcastStreamNamespace()
    {
        return Constants.SnacksBroadcastNamespace;
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
        var snackId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack {snackId} has already been removed.")
                     .Verify(State.IsCreated == false, $"Snack {snackId} already exists.")
                     .Verify(command.Name.IsNotNullOrWhiteSpace(), $"The name of snack {snackId} should not be empty.")
                     .Verify(command.Name.Length <= 100, $"The name of snack {snackId} is too long.")
                     .Verify(command.PictureUrl == null || command.PictureUrl!.Length <= 500, $"The picture url of snack {snackId} is too long.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanInitializeAsync(SnackInitializeCommand command)
    {
        return Task.FromResult(ValidateInitialize(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> InitializeAsync(SnackInitializeCommand command)
    {
        return ValidateInitialize(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => PublishAsync(new SnackInitializedEvent(State.Id, Version, State.Name, State.PictureUrl, command.TraceId, State.CreatedAt ?? DateTimeOffset.UtcNow, State.CreatedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(this.GetPrimaryKey(), Version, 101, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateRemove(SnackRemoveCommand command)
    {
        var snackId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack {snackId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack {snackId} is not initialized.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanRemoveAsync(SnackRemoveCommand command)
    {
        return Task.FromResult(ValidateRemove(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> RemoveAsync(SnackRemoveCommand command)
    {
        return ValidateRemove(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => PublishAsync(new SnackRemovedEvent(State.Id, Version, command.TraceId, State.DeletedAt ?? DateTimeOffset.UtcNow, State.DeletedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(this.GetPrimaryKey(), Version, 102, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateChangeName(SnackChangeNameCommand command)
    {
        var snackId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack {snackId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack {snackId} is not initialized.")
                     .Verify(command.Name.IsNotNullOrWhiteSpace(), $"The name of snack {snackId} should not be empty.")
                     .Verify(command.Name.Length <= 100, $"The name of snack {snackId} is too long.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanChangeNameAsync(SnackChangeNameCommand command)
    {
        return Task.FromResult(ValidateChangeName(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> ChangeNameAsync(SnackChangeNameCommand command)
    {
        return ValidateChangeName(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => PublishAsync(new SnackNameChangedEvent(State.Id, Version, State.Name, command.TraceId, State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(this.GetPrimaryKey(), Version, 103, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateChangePictureUrl(SnackChangePictureUrlCommand command)
    {
        var snackId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack {snackId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack {snackId} is not initialized.")
                     .Verify(command.PictureUrl.IsNullOrWhiteSpace() || command.PictureUrl!.Length <= 500, $"The picture url of snack {snackId} is too long.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanChangePictureUrlAsync(SnackChangePictureUrlCommand command)
    {
        return Task.FromResult(ValidateChangePictureUrl(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> ChangePictureUrlAsync(SnackChangePictureUrlCommand command)
    {
        return ValidateChangePictureUrl(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => PublishAsync(new SnackPictureUrlChangedEvent(State.Id, Version, State.PictureUrl, command.TraceId, State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(this.GetPrimaryKey(), Version, 104, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    #region Persistence

    private async Task PersistAsync()
    {
        var snackInGrain = State;
        var snack = await _dbContext.Snacks.FindAsync(snackInGrain.Id);
        if (snack == null)
        {
            snack = new Snack();
            _dbContext.Snacks.Add(snack);
        }
        snack.Id = snackInGrain.Id;
        snack.Name = snackInGrain.Name;
        snack.PictureUrl = snackInGrain.PictureUrl;
        snack.CreatedAt = snackInGrain.CreatedAt;
        snack.LastModifiedAt = snackInGrain.LastModifiedAt;
        snack.DeletedAt = snackInGrain.DeletedAt;
        snack.CreatedBy = snackInGrain.CreatedBy;
        snack.LastModifiedBy = snackInGrain.LastModifiedBy;
        snack.DeletedBy = snackInGrain.DeletedBy;
        snack.IsDeleted = snackInGrain.IsDeleted;
        await _dbContext.SaveChangesAsync();
    }

    #endregion

}
