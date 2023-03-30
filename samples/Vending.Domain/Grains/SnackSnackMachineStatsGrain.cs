using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

/// <summary>
///     Grain implementation class SnackSnackMachineStatsGrain.
/// </summary>
[StorageProvider(ProviderName = Constants.GrainStorageName)]
public class SnackSnackMachineStatsGrain : StatefulGrainWithGuidKey<SnackMachineStats, SnackEvent, SnackErrorEvent>, ISnackSnackMachineStatsGrain
{
    private readonly DomainDbContext _dbContext;
    private readonly ILogger<SnackSnackMachineStatsGrain> _logger;

    /// <inheritdoc />
    public SnackSnackMachineStatsGrain(DomainDbContext dbContext, ILogger<SnackSnackMachineStatsGrain> logger) : base(Constants.StreamProviderName)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await UpdateCountAsync();
        await base.OnActivateAsync(cancellationToken);
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
    public Task<SnackMachineStats> GetStateAsync()
    {
        return Task.FromResult(State);
    }

    /// <inheritdoc />
    public Task<int> GetCountAsync()
    {
        return Task.FromResult(State.Count);
    }

    private Result ValidateIncrementCount(SnackIncrementMachineCountCommand command)
    {
        var snackId = this.GetPrimaryKey();
        return Result.Ok().Verify(command.Number > 0, $"The number of machines to increment for {snackId} should be greater than 0.").Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    private Task IncrementCountAsync(int number)
    {
        State.Count += number;
        _logger.LogInformation("Incremented count of machines that have this snack to {Count}", State.Count);
        return WriteStateAsync();
    }

    /// <inheritdoc />
    public Task<Result> IncrementCountAsync(SnackIncrementMachineCountCommand command)
    {
        var snackId = this.GetPrimaryKey();
        return ValidateIncrementCount(command)
              .MapTryAsync(() => IncrementCountAsync(command.Number))
              .MapTryAsync(() => PublishAsync(new SnackMachineCountUpdatedEvent(snackId, State.Count, command.TraceId, command.OperatedAt, command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(snackId, 0, 131, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateDecrementCount(SnackDecrementMachineCountCommand command)
    {
        var snackId = this.GetPrimaryKey();
        return Result.Ok().Verify(command.Number > 0, $"The number of machines to decrement for {snackId} should be greater than 0.").Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    private Task DecrementCountAsync(int number)
    {
        State.Count -= number;
        _logger.LogInformation("Decremented count of machines that have this snack to {Count}", State.Count);
        return WriteStateAsync();
    }

    /// <inheritdoc />
    public Task<Result> DecrementCountAsync(SnackDecrementMachineCountCommand command)
    {
        var snackId = this.GetPrimaryKey();
        return ValidateDecrementCount(command)
              .MapTryAsync(() => DecrementCountAsync(command.Number))
              .MapTryAsync(() => PublishAsync(new SnackMachineCountUpdatedEvent(snackId, State.Count, command.TraceId, command.OperatedAt, command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(snackId, 0, 132, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    #region Update From DB

    private async Task UpdateCountAsync()
    {
        var snackId = this.GetPrimaryKey();
        try
        {
            var count = await _dbContext.SnackMachines.Where(sm => sm.IsDeleted == false && sm.Slots.Any(sl => sl.SnackPile != null && sl.SnackPile.SnackId == snackId)).CountAsync();
            if (State.Count != count)
            {
                State.Count = count;
                _logger.LogInformation("Updated count of machines that have this snack from {OldCount} to {NewCount}", State.Count, count);
                await WriteStateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update count for snack {SnackId}", snackId);
        }
    }

    #endregion

}
