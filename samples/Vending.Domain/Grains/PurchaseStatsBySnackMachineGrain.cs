using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orleans.Providers;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Events;
using Vending.Domain.Abstractions.Grains;
using Vending.Domain.Abstractions.States;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain.Grains;

/// <summary>
///     Grain implementation class PurchaseStatsBySnackMachineGrain.
/// </summary>
[StorageProvider(ProviderName = Constants.GrainStorageName)]
public class PurchaseStatsBySnackMachineGrain : StatefulGrainWithGuidKey<PurchaseStats, SnackMachineEvent, SnackMachineErrorEvent>, IPurchaseStatsBySnackMachineGrain
{
    private readonly DomainDbContext _dbContext;
    private readonly ILogger<PurchaseStatsBySnackMachineGrain> _logger;

    /// <inheritdoc />
    public PurchaseStatsBySnackMachineGrain(DomainDbContext dbContext, ILogger<PurchaseStatsBySnackMachineGrain> logger) : base(Constants.StreamProviderName)
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

    /// <inheritdoc />
    public Task IncrementCountAsync(int number)
    {
        State.Count += number;
        _logger.LogInformation("Incremented count of purchases that have this snack machine to {Count}", State.Count);
        return WriteStateAsync();
    }

    public Task DecrementCountAsync(int number)
    {
        State.Count -= number;
        _logger.LogInformation("Decremented count of purchases that have this snack machine to {Count}", State.Count);
        return WriteStateAsync();
    }

    /// <inheritdoc />
    public Task<decimal> GetAmountAsync()
    {
        return Task.FromResult(State.Amount);
    }

    /// <inheritdoc />
    public Task IncrementAmountAsync(decimal amount)
    {
        State.Amount += amount;
        _logger.LogInformation("Incremented amount of purchases that have this snack machine to {Amount}", State.Amount);
        return WriteStateAsync();
    }

    /// <inheritdoc />
    public Task DecrementAmountAsync(decimal amount)
    {
        State.Amount -= amount;
        _logger.LogInformation("Decremented amount of purchases that have this snack machine to {Amount}", State.Amount);
        return WriteStateAsync();
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
                _logger.LogInformation("Updated count of purchases that have this snack machine from {OldCount} to {NewCount}", State.Count, count);
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
                _logger.LogInformation("Updated amount of purchases that have this snack machine from {OldAmount} to {NewAmount}", State.Amount, amount);
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
