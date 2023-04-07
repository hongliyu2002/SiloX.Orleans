using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orleans.FluentResults;
using Orleans.Providers;
using SiloX.Domain.Abstractions;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;
using Vending.Domain.Abstractions.Snacks;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain.Snacks;

/// <summary>
///     Grain implementation class SnackStatsOfMachinesGrain.
/// </summary>
[StorageProvider(ProviderName = Constants.GrainStorageName)]
public class SnackStatsOfMachinesGrain : StatefulGrainWithGuidKey<StatsOfMachines, SnackEvent, SnackErrorEvent>, ISnackStatsOfMachinesGrain
{
    private readonly DomainDbContext _dbContext;
    private readonly ILogger<SnackStatsOfMachinesGrain> _logger;

    /// <inheritdoc />
    public SnackStatsOfMachinesGrain(DomainDbContext dbContext, ILogger<SnackStatsOfMachinesGrain> logger)
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
    public Task<StatsOfMachines> GetStatsOfMachinesAsync()
    {
        return Task.FromResult(State);
    }

    /// <inheritdoc />
    public Task<int> GetMachineCountAsync()
    {
        return Task.FromResult(State.MachineCount);
    }

    /// <inheritdoc />
    public Task<int> GetTotalQuantityAsync()
    {
        return Task.FromResult(State.TotalQuantity);
    }

    /// <inheritdoc />
    public Task<decimal> GetTotalAmountAsync()
    {
        return Task.FromResult(State.TotalAmount);
    }

    /// <inheritdoc />
    public Task<Result> UpdateMachineCountAsync(int machineCount)
    {
        var snackId = this.GetPrimaryKey();
        var traceId = Guid.NewGuid();
        var operationAt = DateTimeOffset.UtcNow;
        var operationBy = $"System/{GetType().Name}";
        return Result.Ok()
                     .MapTryAsync(() => ApplyMachineCountAsync(machineCount))
                     .MapTryAsync(() => PublishAsync(new SnackMachineCountUpdatedEvent(snackId, State.MachineCount, traceId, operationAt, operationBy)))
                     .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(snackId, 0, 131, errors.ToReasonStrings(), traceId, operationAt, operationBy)));
    }

    /// <inheritdoc />
    public Task<Result> UpdateTotalQuantityAsync(int totalQuantity)
    {
        var snackId = this.GetPrimaryKey();
        var traceId = Guid.NewGuid();
        var operationAt = DateTimeOffset.UtcNow;
        var operationBy = $"System/{GetType().Name}";
        return Result.Ok()
                     .MapTryAsync(() => ApplyTotalQuantityAsync(totalQuantity))
                     .MapTryAsync(() => PublishAsync(new SnackTotalQuantityUpdatedEvent(snackId, State.TotalQuantity, traceId, operationAt, operationBy)))
                     .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(snackId, 0, 132, errors.ToReasonStrings(), traceId, operationAt, operationBy)));
    }

    /// <inheritdoc />
    public Task<Result> UpdateTotalAmountAsync(decimal totalAmount)
    {
        var snackId = this.GetPrimaryKey();
        var traceId = Guid.NewGuid();
        var operationAt = DateTimeOffset.UtcNow;
        var operationBy = $"System/{GetType().Name}";
        return Result.Ok()
                     .MapTryAsync(() => ApplyTotalAmountAsync(totalAmount))
                     .MapTryAsync(() => PublishAsync(new SnackTotalAmountUpdatedEvent(snackId, State.TotalAmount, traceId, operationAt, operationBy)))
                     .TapErrorTryAsync(errors => PublishErrorAsync(new SnackErrorEvent(snackId, 0, 133, errors.ToReasonStrings(), traceId, operationAt, operationBy)));
    }

    #region Persistence

    private async Task ApplyMachineCountAsync(int machineCount)
    {
        if (machineCount < 0)
        {
            var snackId = this.GetPrimaryKey();
            machineCount = await _dbContext.Machines.Where(m => m.IsDeleted == false && m.SnackStats.Any(ss => ss.SnackId == snackId)).CountAsync();
        }
        if (State.MachineCount != machineCount)
        {
            State.MachineCount = machineCount;
            await WriteStateAsync();
            _logger.LogInformation("Updated count of machines that have this snack from {OldCount} to {NewCount}", State.MachineCount, machineCount);
        }
    }

    private async Task ApplyTotalQuantityAsync(int totalQuantity)
    {
        if (totalQuantity < 0)
        {
            var snackId = this.GetPrimaryKey();
            totalQuantity = await _dbContext.Machines.Where(m => m.IsDeleted == false).SelectMany(m => m.SnackStats).Where(ss => ss.SnackId == snackId).SumAsync(ss => ss.TotalQuantity);
        }
        if (State.TotalQuantity != totalQuantity)
        {
            State.TotalQuantity = totalQuantity;
            await WriteStateAsync();
            _logger.LogInformation("Updated quantity of this snack from {OldCQuantity} to {NewQuantity}", State.TotalQuantity, totalQuantity);
        }
    }

    private async Task ApplyTotalAmountAsync(decimal totalAmount)
    {
        if (totalAmount < 0)
        {
            var snackId = this.GetPrimaryKey();
            totalAmount = await _dbContext.Machines.Where(m => m.IsDeleted == false).SelectMany(m => m.SnackStats).Where(ss => ss.SnackId == snackId).SumAsync(ss => ss.TotalAmount);
        }
        if (State.TotalAmount != totalAmount)
        {
            State.TotalAmount = totalAmount;
            await WriteStateAsync();
            _logger.LogInformation("Updated amount of this snack from {OldCAmount} to {NewAmount}", State.TotalAmount, totalAmount);
        }
    }

    #endregion

}
