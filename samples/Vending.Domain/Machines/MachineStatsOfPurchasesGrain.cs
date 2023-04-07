﻿using Fluxera.Guards;
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
    protected override string GetStreamNamespace()
    {
        return Constants.MachinesNamespace;
    }

    /// <inheritdoc />
    protected override string GetBroadcastStreamNamespace()
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
        var operationAt = DateTimeOffset.UtcNow;
        var operationBy = $"System/{GetType().Name}";
        return Result.Ok()
                     .MapTryAsync(() => ApplyBoughtCountAsync(boughtCount))
                     .MapTryAsync(() => PublishAsync(new MachineBoughtCountUpdatedEvent(machineId, State.BoughtCount, traceId, operationAt, operationBy)))
                     .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(machineId, 0, 221, errors.ToReasonStrings(), traceId, operationAt, operationBy)));
    }

    /// <inheritdoc />
    public Task<Result> UpdateBoughtAmountAsync(decimal boughtAmount)
    {
        var machineId = this.GetPrimaryKey();
        var traceId = Guid.NewGuid();
        var operationAt = DateTimeOffset.UtcNow;
        var operationBy = $"System/{GetType().Name}";
        return Result.Ok()
                     .MapTryAsync(() => ApplyBoughtAmountAsync(boughtAmount))
                     .MapTryAsync(() => PublishAsync(new MachineBoughtAmountUpdatedEvent(machineId, State.BoughtAmount, traceId, operationAt, operationBy)))
                     .TapErrorTryAsync(errors => PublishErrorAsync(new MachineErrorEvent(machineId, 0, 222, errors.ToReasonStrings(), traceId, operationAt, operationBy)));
    }

    #region Persistence

    private async Task ApplyBoughtCountAsync(int boughtCount)
    {
        if (boughtCount < 0)
        {
            var machineId = this.GetPrimaryKey();
            boughtCount = await _dbContext.Purchases.Where(p => p.MachineId == machineId).CountAsync();
        }
        if (State.BoughtCount != boughtCount)
        {
            State.BoughtCount = boughtCount;
            await WriteStateAsync();
            _logger.LogInformation("Updated count of purchases that made for this machine from {OldCount} to {NewCount}", State.BoughtCount, boughtCount);
        }
    }

    private async Task ApplyBoughtAmountAsync(decimal boughtAmount)
    {
        if (boughtAmount < 0)
        {
            var machineId = this.GetPrimaryKey();
            boughtAmount = await _dbContext.Purchases.Where(p => p.MachineId == machineId).SumAsync(p => p.BoughtPrice);
        }
        if (State.BoughtAmount != boughtAmount)
        {
            State.BoughtAmount = boughtAmount;
            await WriteStateAsync();
            _logger.LogInformation("Updated amount of purchases that made for this machine from {OldAmount} to {NewAmount}", State.BoughtAmount, boughtAmount);
        }
    }

    #endregion

}