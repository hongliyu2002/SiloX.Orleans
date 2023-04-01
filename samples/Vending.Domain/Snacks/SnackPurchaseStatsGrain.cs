using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orleans.FluentResults;
using Orleans.Providers;
using SiloX.Domain.Abstractions;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Purchases;
using Vending.Domain.Abstractions.Snacks;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain.Snacks;

/// <summary>
///     Grain implementation class SnackPurchaseStatsGrain.
/// </summary>
[StorageProvider(ProviderName = Constants.GrainStorageName)]
public class SnackPurchaseStatsGrain : StatefulGrainWithGuidKey<PurchaseStats, SnackEvent, SnackErrorEvent>, ISnackPurchaseStatsGrain
{
    private readonly DomainDbContext _dbContext;
    private readonly ILogger<SnackPurchaseStatsGrain> _logger;

    /// <inheritdoc />
    public SnackPurchaseStatsGrain(DomainDbContext dbContext, ILogger<SnackPurchaseStatsGrain> logger)
        : base(Constants.StreamProviderName)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await UpdateCountAsync();
        await UpdateAmountAsync();
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
    public Task<PurchaseStats> GetStateAsync()
    {
        return Task.FromResult(State);
    }

    /// <inheritdoc />
    public Task<int> GetCountAsync()
    {
        return Task.FromResult(State.Count);
    }

    private Result ValidateIncrementCount(SnackIncrementBoughtCountCommand command)
    {
        var snackId = this.GetPrimaryKey();
        return Result.Ok().Verify(command.Number > 0, $"The number of purchases to increment for {snackId} should be greater than 0.").Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    private Task IncrementCountAsync(int number)
    {
        State.Count += number;
        _logger.LogInformation("Incremented count of purchases that have this snack to {Count}", State.Count);
        return WriteStateAsync();
    }

    /// <inheritdoc />
    public Task<Result> IncrementCountAsync(SnackIncrementBoughtCountCommand command)
    {
        var snackId = this.GetPrimaryKey();
        return ValidateIncrementCount(command)
              .MapTryAsync(() => IncrementCountAsync(command.Number))
              .MapTryAsync(() => PublishAsync(new SnackBoughtCountUpdatedEvent(snackId, State.Count, command.TraceId, command.OperatedAt, command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(snackId, 0, 121, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateDecrementCount(SnackDecrementBoughtCountCommand command)
    {
        var snackId = this.GetPrimaryKey();
        return Result.Ok().Verify(command.Number > 0, $"The number of purchases to decrement for {snackId} should be greater than 0.").Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    private Task DecrementCountAsync(int number)
    {
        State.Count -= number;
        _logger.LogInformation("Decremented count of purchases that have this snack to {Count}", State.Count);
        return WriteStateAsync();
    }

    /// <inheritdoc />
    public Task<Result> DecrementCountAsync(SnackDecrementBoughtCountCommand command)
    {
        var snackId = this.GetPrimaryKey();
        return ValidateDecrementCount(command)
              .MapTryAsync(() => DecrementCountAsync(command.Number))
              .MapTryAsync(() => PublishAsync(new SnackBoughtCountUpdatedEvent(snackId, State.Count, command.TraceId, command.OperatedAt, command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(snackId, 0, 122, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    /// <inheritdoc />
    public Task<decimal> GetAmountAsync()
    {
        return Task.FromResult(State.Amount);
    }

    private Result ValidateIncrementAmount(SnackIncrementBoughtAmountCommand command)
    {
        var snackId = this.GetPrimaryKey();
        return Result.Ok().Verify(command.Amount >= 0, $"The amount of purchases to increment for {snackId} should be greater than or equals 0.").Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    private Task IncrementAmountAsync(decimal amount)
    {
        State.Amount += amount;
        _logger.LogInformation("Incremented amount of purchases that have this snack to {Amount}", State.Amount);
        return WriteStateAsync();
    }

    /// <inheritdoc />
    public Task<Result> IncrementAmountAsync(SnackIncrementBoughtAmountCommand command)
    {
        var snackId = this.GetPrimaryKey();
        return ValidateIncrementAmount(command)
              .MapTryAsync(() => IncrementAmountAsync(command.Amount))
              .MapTryAsync(() => PublishAsync(new SnackBoughtAmountUpdatedEvent(snackId, State.Amount, command.TraceId, command.OperatedAt, command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(snackId, 0, 123, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    private Result ValidateDecrementAmount(SnackDecrementBoughtAmountCommand command)
    {
        var snackId = this.GetPrimaryKey();
        return Result.Ok().Verify(command.Amount >= 0, $"The amount of purchases to decrement for {snackId} should be greater than or equals 0.").Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    private Task DecrementAmountAsync(decimal amount)
    {
        State.Amount -= amount;
        _logger.LogInformation("Decremented amount of purchases that have this snack to {Amount}", State.Amount);
        return WriteStateAsync();
    }

    /// <inheritdoc />
    public Task<Result> DecrementAmountAsync(SnackDecrementBoughtAmountCommand command)
    {
        var snackId = this.GetPrimaryKey();
        return ValidateDecrementAmount(command)
              .MapTryAsync(() => DecrementAmountAsync(command.Amount))
              .MapTryAsync(() => PublishAsync(new SnackBoughtAmountUpdatedEvent(snackId, State.Amount, command.TraceId, command.OperatedAt, command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(snackId, 0, 124, errors.ToReasonStrings(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    #region Update From DB

    private async Task UpdateCountAsync()
    {
        var snackId = this.GetPrimaryKey();
        try
        {
            var count = await _dbContext.Purchases.Where(p => p.SnackId == snackId).CountAsync();
            if (State.Count != count)
            {
                State.Count = count;
                _logger.LogInformation("Updated count of purchases that have this snack from {OldCount} to {NewCount}", State.Count, count);
                await WriteStateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update count of purchases for snack {SnackId}", snackId);
        }
    }

    private async Task UpdateAmountAsync()
    {
        var snackId = this.GetPrimaryKey();
        try
        {
            var amount = await _dbContext.Purchases.Where(p => p.SnackId == snackId).SumAsync(p => p.BoughtPrice);
            if (State.Amount != amount)
            {
                State.Amount = amount;
                _logger.LogInformation("Updated amount of purchases that have this snack from {OldAmount} to {NewAmount}", State.Amount, amount);
                await WriteStateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update amount of purchases for snack {SnackId}", snackId);
        }
    }

    #endregion

}
