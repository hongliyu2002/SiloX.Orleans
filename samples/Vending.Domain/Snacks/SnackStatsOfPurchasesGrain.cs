using Fluxera.Guards;
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
///     Grain implementation class SnackStatsOfPurchasesGrain.
/// </summary>
[StorageProvider(ProviderName = Constants.GrainStorageName)]
public class SnackStatsOfPurchasesGrain : StatefulGrainWithGuidKey<StatsOfPurchases, SnackEvent, SnackErrorEvent>, ISnackStatsOfPurchasesGrain
{
    private readonly DomainDbContext _dbContext;
    private readonly ILogger<SnackStatsOfPurchasesGrain> _logger;

    /// <inheritdoc />
    public SnackStatsOfPurchasesGrain(DomainDbContext dbContext, ILogger<SnackStatsOfPurchasesGrain> logger)
        : base(Constants.StreamProviderName)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _logger = Guard.Against.Null(logger, nameof(logger));
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
    public Task<StatsOfPurchases> GetStatsOfPurchasesAsync()
    {
        return Task.FromResult(State);
    }

    /// <inheritdoc />
    public Task<int> GetBoughtCountAsync()
    {
        return Task.FromResult(State.BoughtCount);
    }

    /// <inheritdoc />
    public Task<decimal> GetBoughtAmountAsync()
    {
        return Task.FromResult(State.BoughtAmount);
    }

    /// <inheritdoc />
    public Task<Result> UpdateBoughtCountAsync(int boughtCount)
    {
        var snackId = this.GetPrimaryKey();
        var traceId = Guid.NewGuid();
        var operationAt = DateTimeOffset.UtcNow;
        var operationBy = $"System/{GetType().Name}";
        return Result.Ok()
                     .MapTryAsync(() => ApplyBoughtCountAsync(boughtCount))
                     .MapTryAsync(() => PublishAsync(new SnackBoughtCountUpdatedEvent(snackId, State.BoughtCount, traceId, operationAt, operationBy)))
                     .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(snackId, 0, 121, errors.ToReasonStrings(), traceId, operationAt, operationBy)));
    }

    /// <inheritdoc />
    public Task<Result> UpdateBoughtAmountAsync(decimal boughtAmount)
    {
        var snackId = this.GetPrimaryKey();
        var traceId = Guid.NewGuid();
        var operationAt = DateTimeOffset.UtcNow;
        var operationBy = $"System/{GetType().Name}";
        return Result.Ok()
                     .MapTryAsync(() => ApplyBoughtAmountAsync(boughtAmount))
                     .MapTryAsync(() => PublishAsync(new SnackBoughtAmountUpdatedEvent(snackId, State.BoughtAmount, traceId, operationAt, operationBy)))
                     .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(snackId, 0, 122, errors.ToReasonStrings(), traceId, operationAt, operationBy)));
    }

    #region Persistence

    private async Task ApplyBoughtCountAsync(int boughtCount)
    {
        if (boughtCount < 0)
        {
            var snackId = this.GetPrimaryKey();
            boughtCount = await _dbContext.Purchases.Where(p => p.SnackId == snackId).CountAsync();
        }
        if (State.BoughtCount != boughtCount)
        {
            State.BoughtCount = boughtCount;
            await WriteStateAsync();
            _logger.LogInformation("Updated count of purchases that have this snack from {OldCount} to {NewCount}", State.BoughtCount, boughtCount);
        }
    }

    private async Task ApplyBoughtAmountAsync(decimal boughtAmount)
    {
        if (boughtAmount < 0)
        {
            var snackId = this.GetPrimaryKey();
            boughtAmount = await _dbContext.Purchases.Where(p => p.SnackId == snackId).SumAsync(p => p.BoughtPrice);
        }
        if (State.BoughtAmount != boughtAmount)
        {
            State.BoughtAmount = boughtAmount;
            await WriteStateAsync();
            _logger.LogInformation("Updated amount of purchases that have this snack from {OldAmount} to {NewAmount}", State.BoughtAmount, boughtAmount);
        }
    }

    #endregion

}
