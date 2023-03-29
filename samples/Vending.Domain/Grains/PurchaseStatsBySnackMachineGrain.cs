using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Grains;
using Vending.Domain.Abstractions.States;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain.Grains;

/// <summary>
///     Grain implementation class PurchaseStatsBySnackMachineGrain.
/// </summary>
public class PurchaseStatsBySnackMachineGrain : Grain, IPurchaseStatsBySnackMachineGrain
{
    private readonly IPersistentState<PurchaseStats> _stats;
    private readonly DomainDbContext _dbContext;
    private readonly ILogger<PurchaseStatsBySnackMachineGrain> _logger;

    /// <inheritdoc />
    public PurchaseStatsBySnackMachineGrain([PersistentState(nameof(PurchaseStats), Constants.GrainStorageName2)] IPersistentState<PurchaseStats> stats, DomainDbContext dbContext, ILogger<PurchaseStatsBySnackMachineGrain> logger)
    {
        _stats = Guard.Against.Null(stats, nameof(stats));
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
        await UpdateCountAsync();
        await UpdateAmountAsync();
    }

    /// <inheritdoc />
    public Task<PurchaseStats> GetStateAsync()
    {
        return Task.FromResult(_stats.State);
    }

    /// <inheritdoc />
    public Task<int> GetCountAsync()
    {
        return Task.FromResult(_stats.State.Count);
    }

    /// <inheritdoc />
    public Task IncrementCountAsync(int number)
    {
        _stats.State.Count += number;
        _logger.LogInformation("Incremented count of purchases that have this snack machine to {Count}", _stats.State.Count);
        return _stats.WriteStateAsync();
    }

    public Task DecrementCountAsync(int number)
    {
        _stats.State.Count -= number;
        _logger.LogInformation("Decremented count of purchases that have this snack machine to {Count}", _stats.State.Count);
        return _stats.WriteStateAsync();
    }

    /// <inheritdoc />
    public Task<decimal> GetAmountAsync()
    {
        return Task.FromResult(_stats.State.Amount);
    }

    /// <inheritdoc />
    public Task IncrementAmountAsync(decimal amount)
    {
        _stats.State.Amount += amount;
        _logger.LogInformation("Incremented amount of purchases that have this snack machine to {Amount}", _stats.State.Amount);
        return _stats.WriteStateAsync();
    }

    /// <inheritdoc />
    public Task DecrementAmountAsync(decimal amount)
    {
        _stats.State.Amount -= amount;
        _logger.LogInformation("Decremented amount of purchases that have this snack machine to {Amount}", _stats.State.Amount);
        return _stats.WriteStateAsync();
    }

    #region Update From DB

    private async Task UpdateCountAsync()
    {
        try
        {
            var machineId = this.GetPrimaryKey();
            var countInDb = await _dbContext.Purchases.Where(p => p.MachineId == machineId).CountAsync();
            if (_stats.State.Count != countInDb)
            {
                _logger.LogInformation("Updated count of purchases that have this snack machine from {OldCount} to {NewCount}", _stats.State.Count, countInDb);
                _stats.State.Count = countInDb;
                await _stats.WriteStateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update count for snack machine {MachineId}", this.GetPrimaryKey());
        }
    }

    private async Task UpdateAmountAsync()
    {
        try
        {
            var machineId = this.GetPrimaryKey();
            var amountInDb = await _dbContext.Purchases.Where(p => p.MachineId == machineId).SumAsync(p => p.BoughtPrice);
            if (_stats.State.Amount != amountInDb)
            {
                _logger.LogInformation("Updated amount of purchases that have this snack machine from {OldAmount} to {NewAmount}", _stats.State.Amount, amountInDb);
                _stats.State.Amount = amountInDb;
                await _stats.WriteStateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update amount for snack machine {MachineId}", this.GetPrimaryKey());
        }
    }

    #endregion

}
