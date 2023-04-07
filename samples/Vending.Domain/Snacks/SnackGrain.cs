using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using Orleans.FluentResults;
using Orleans.Providers;
using SiloX.Domain.Abstractions;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Snacks;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain.Snacks;

[LogConsistencyProvider(ProviderName = Constants.LogConsistencyName)]
[StorageProvider(ProviderName = Constants.GrainStorageName)]
public sealed class SnackGrain : EventSourcingGrainWithGuidKey<Snack, SnackCommand, SnackEvent, SnackErrorEvent>, ISnackGrain
{
    private readonly DomainDbContext _dbContext;

    /// <inheritdoc />
    public SnackGrain(DomainDbContext dbContext)
        : base(Constants.StreamProviderName)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
    }

    /// <inheritdoc />
    protected override string GetPubStreamNamespace()
    {
        return Constants.SnacksNamespace;
    }

    /// <inheritdoc />
    protected override string GetPubBroadcastStreamNamespace()
    {
        return Constants.SnacksBroadcastNamespace;
    }

    /// <inheritdoc />
    public Task<Snack> GetSnackAsync()
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
                     .Verify(command.Name.Length <= 200, $"The name of snack {snackId} is too long.")
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
              .MapTryAsync(() => PublishAsync(new SnackInitializedEvent(State.Id, Version, State.Name, State.PictureUrl, command.TraceId,
                                                                        State.CreatedAt ?? DateTimeOffset.UtcNow, State.CreatedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(this.GetPrimaryKey(), Version, 101, errors.ToReasonStrings(), command.TraceId,
                                                                                DateTimeOffset.UtcNow, command.OperatedBy)));
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
              .MapTryAsync(() => PublishAsync(new SnackRemovedEvent(State.Id, Version, command.TraceId, State.DeletedAt ?? DateTimeOffset.UtcNow,
                                                                    State.DeletedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(this.GetPrimaryKey(), Version, 102, errors.ToReasonStrings(), command.TraceId,
                                                                                DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateUpdate(SnackUpdateCommand command)
    {
        var snackId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsDeleted == false, $"Snack {snackId} has already been removed.")
                     .Verify(State.IsCreated, $"Snack {snackId} is not initialized.")
                     .Verify(command.Name.IsNotNullOrWhiteSpace(), $"The name of snack {snackId} should not be empty.")
                     .Verify(command.Name.Length <= 200, $"The name of snack {snackId} is too long.")
                     .Verify(command.PictureUrl.IsNullOrWhiteSpace() || command.PictureUrl!.Length <= 500, $"The picture url of snack {snackId} is too long.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanUpdateAsync(SnackUpdateCommand command)
    {
        return Task.FromResult(ValidateUpdate(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> UpdateAsync(SnackUpdateCommand command)
    {
        return ValidateUpdate(command)
              .MapTryAsync(() => RaiseConditionalEvent(command))
              .MapTryIfAsync(persisted => persisted, PersistAsync)
              .MapTryAsync(() => PublishAsync(new SnackUpdatedEvent(State.Id, Version, State.Name, State.PictureUrl, command.TraceId,
                                                                    State.LastModifiedAt ?? DateTimeOffset.UtcNow, State.LastModifiedBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(this.GetPrimaryKey(), Version, 103, errors.ToReasonStrings(), command.TraceId,
                                                                                DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    #region Persistence

    private async Task PersistAsync()
    {
        var snack = await _dbContext.Snacks.FirstOrDefaultAsync(s => s.Id == State.Id);
        if (snack != null)
        {
            _dbContext.Entry(snack).CurrentValues.SetValues(State);
        }
        else
        {
            _dbContext.Snacks.Add(State);
        }
        await _dbContext.SaveChangesAsync();
    }

    #endregion

}
