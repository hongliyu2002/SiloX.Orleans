using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orleans.FluentResults;
using Orleans.Providers;
using SiloX.Domain.Abstractions;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;
using Vending.Domain.Abstractions.Purchases;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain.Machines;

/// <summary>
///     Grain implementation class MachineStatsOfPurchasesGrain.
/// </summary>
[StorageProvider(ProviderName = Constants.GrainStorageName)]
public class MachineStatsOfPurchasesGrain : StatefulGrainWithGuidKey<StatsOfPurchases, MachineEvent, MachineErrorEvent>, IMachineStatsOfPurchasesGrain
{
    private readonly DomainDbContext _dbContext;
    private readonly ILogger<MachineStatsOfPurchasesGrain> _logger;

    /// <inheritdoc />
    public MachineStatsOfPurchasesGrain(DomainDbContext dbContext, ILogger<MachineStatsOfPurchasesGrain> logger)
        : base(Constants.StreamProviderName)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    protected override string GetPubStreamNamespace()
    {
        return Constants.MachinesNamespace;
    }

    /// <inheritdoc />
    protected override string GetPubBroadcastStreamNamespace()
    {
        return Constants.MachinesBroadcastNamespace;
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
        var machineId = this.GetPrimaryKey();
        var traceId = Guid.NewGuid();
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = $"System/{GetType().Name}";
        return Result.Ok()
                     .MapTryAsync(() => ApplyBoughtCountAsync(boughtCount))
                     .TapTryAsync(() => PublishAsync(new MachineBoughtCountUpdatedEvent(machineId, State.BoughtCount, traceId, operatedAt, operatedBy)))
                     .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(machineId, 0, 221, errors.ToListMessages(), traceId, operatedAt, operatedBy)));
    }

    /// <inheritdoc />
    public Task<Result> UpdateBoughtAmountAsync(decimal boughtAmount)
    {
        var machineId = this.GetPrimaryKey();
        var traceId = Guid.NewGuid();
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = $"System/{GetType().Name}";
        return Result.Ok()
                     .MapTryAsync(() => ApplyBoughtAmountAsync(boughtAmount))
                     .TapTryAsync(() => PublishAsync(new MachineBoughtAmountUpdatedEvent(machineId, State.BoughtAmount, traceId, operatedAt, operatedBy)))
                     .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(machineId, 0, 222, errors.ToListMessages(), traceId, operatedAt, operatedBy)));
    }

    #region Persistence

    private async Task ApplyBoughtCountAsync(int boughtCount)
    {
        var machineId = this.GetPrimaryKey();
        if (boughtCount < 0)
        {
            boughtCount = await _dbContext.Purchases.Where(p => p.MachineId == machineId).CountAsync();
        }
        var oldBoughtCount = State.BoughtCount;
        if (oldBoughtCount != boughtCount)
        {
            State.BoughtCount = boughtCount;
            await WriteStateAsync();
            _logger.LogInformation("Updated count of purchases that made for this machine {MachineId} from {OldCount} to {NewCount}", machineId, oldBoughtCount, boughtCount);
        }
    }

    private async Task ApplyBoughtAmountAsync(decimal boughtAmount)
    {
        var machineId = this.GetPrimaryKey();
        if (boughtAmount < 0)
        {
            boughtAmount = await _dbContext.Purchases.Where(p => p.MachineId == machineId).SumAsync(p => p.BoughtPrice);
        }
        var oldBoughtAmount = State.BoughtAmount;
        if (oldBoughtAmount != boughtAmount)
        {
            State.BoughtAmount = boughtAmount;
            await WriteStateAsync();
            _logger.LogInformation("Updated amount of purchases that made for this machine {MachineId} from {OldAmount} to {NewAmount}", machineId, oldBoughtAmount, boughtAmount);
        }
    }

    #endregion

}