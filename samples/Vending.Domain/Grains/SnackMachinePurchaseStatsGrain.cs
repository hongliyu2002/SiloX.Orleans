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
///     Grain implementation class SnackMachinePurchaseStatsGrain.
/// </summary>
[StorageProvider(ProviderName = Constants.GrainStorageName)]
public class SnackMachinePurchaseStatsGrain : StatefulGrainWithGuidKey<PurchaseStats, SnackMachineEvent, SnackMachineErrorEvent>, ISnackMachinePurchaseStatsGrain
{
    private readonly DomainDbContext _dbContext;
    private readonly ILogger<SnackMachinePurchaseStatsGrain> _logger;

    /// <inheritdoc />
    public SnackMachinePurchaseStatsGrain(DomainDbContext dbContext, ILogger<SnackMachinePurchaseStatsGrain> logger) : base(Constants.StreamProviderName)
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
        return Constants.SnackMachinesNamespace;
    }

    /// <inheritdoc />
    protected override string GetBroadcastStreamNamespace()
    {
        return Constants.SnackMachinesBroadcastNamespace;
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

    private Result ValidateIncrementCount(SnackMachineIncrementBoughtCountCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok().Verify(command.Number > 0, $"The number of boughts to increment for {machineId} should be greater than 0.").Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    private Task IncrementCountAsync(int number)
    {
        State.Count += number;
        _logger.LogInformation("Incremented count of boughts that have this snack machine to {Count}", State.Count);
        return WriteStateAsync();
    }

    /// <inheritdoc />
    public Task<Result> IncrementCountAsync(SnackMachineIncrementBoughtCountCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return ValidateIncrementCount(command)
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackMachineErrorEvent(machineId, 0, 221, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .MapTryAsync(() => IncrementCountAsync(command.Number))
              .MapTryAsync(() => PublishAsync(new SnackMachineBoughtCountUpdatedEvent(machineId, State.Count, command.TraceId, command.OperatedAt, command.OperatedBy)));
    }

    private Result ValidateDecrementCount(SnackMachineDecrementBoughtCountCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok().Verify(command.Number > 0, $"The number of boughts to decrement for {machineId} should be greater than 0.").Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    private Task DecrementCountAsync(int number)
    {
        State.Count -= number;
        _logger.LogInformation("Decremented count of boughts that have this snack machine to {Count}", State.Count);
        return WriteStateAsync();
    }

    /// <inheritdoc />
    public Task<Result> DecrementCountAsync(SnackMachineDecrementBoughtCountCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return ValidateDecrementCount(command)
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackMachineErrorEvent(machineId, 0, 222, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .MapTryAsync(() => DecrementCountAsync(command.Number))
              .MapTryAsync(() => PublishAsync(new SnackMachineBoughtCountUpdatedEvent(machineId, State.Count, command.TraceId, command.OperatedAt, command.OperatedBy)));
    }

    /// <inheritdoc />
    public Task<decimal> GetAmountAsync()
    {
        return Task.FromResult(State.Amount);
    }

    private Result ValidateIncrementAmount(SnackMachineIncrementBoughtAmountCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok().Verify(command.Amount >= 0, $"The amount of boughts to increment for {machineId} should be greater than or equals 0.").Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    private Task IncrementAmountAsync(decimal amount)
    {
        State.Amount += amount;
        _logger.LogInformation("Incremented amount of boughts that have this snack machine to {Amount}", State.Amount);
        return WriteStateAsync();
    }

    /// <inheritdoc />
    public Task<Result> IncrementAmountAsync(SnackMachineIncrementBoughtAmountCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return ValidateIncrementAmount(command)
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackMachineErrorEvent(machineId, 0, 223, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .MapTryAsync(() => IncrementAmountAsync(command.Amount))
              .MapTryAsync(() => PublishAsync(new SnackMachineBoughtAmountUpdatedEvent(machineId, State.Amount, command.TraceId, command.OperatedAt, command.OperatedBy)));
    }

    private Result ValidateDecrementAmount(SnackMachineDecrementBoughtAmountCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return Result.Ok().Verify(command.Amount >= 0, $"The amount of boughts to decrement for {machineId} should be greater than or equals 0.").Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    private Task DecrementAmountAsync(decimal amount)
    {
        State.Amount -= amount;
        _logger.LogInformation("Decremented amount of boughts that have this snack machine to {Amount}", State.Amount);
        return WriteStateAsync();
    }

    /// <inheritdoc />
    public Task<Result> DecrementAmountAsync(SnackMachineDecrementBoughtAmountCommand command)
    {
        var machineId = this.GetPrimaryKey();
        return ValidateDecrementAmount(command)
              .TapErrorTryAsync(errors => PublishErrorAsync(new SnackMachineErrorEvent(machineId, 0, 224, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .MapTryAsync(() => DecrementAmountAsync(command.Amount))
              .MapTryAsync(() => PublishAsync(new SnackMachineBoughtAmountUpdatedEvent(machineId, State.Amount, command.TraceId, command.OperatedAt, command.OperatedBy)));
    }

    #region Update From DB

    private async Task UpdateCountAsync()
    {
        var machineId = this.GetPrimaryKey();
        try
        {
            var count = await _dbContext.Purchases.Where(p => p.MachineId == machineId).CountAsync();
            if (State.Count != count)
            {
                State.Count = count;
                _logger.LogInformation("Updated count of boughts that have this snack machine from {OldCount} to {NewCount}", State.Count, count);
                await WriteStateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update count for snack machine {MachineId}", machineId);
        }
    }

    private async Task UpdateAmountAsync()
    {
        var machineId = this.GetPrimaryKey();
        try
        {
            var amount = await _dbContext.Purchases.Where(p => p.MachineId == machineId).SumAsync(p => p.BoughtPrice);
            if (State.Amount != amount)
            {
                State.Amount = amount;
                _logger.LogInformation("Updated amount of boughts that have this snack machine from {OldAmount} to {NewAmount}", State.Amount, amount);
                await WriteStateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update amount for snack machine {MachineId}", machineId);
        }
    }

    #endregion

}
