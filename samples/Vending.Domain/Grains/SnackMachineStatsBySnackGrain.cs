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
///     Grain implementation class SnackMachineStatsBySnackGrain.
/// </summary>
public class SnackMachineStatsBySnackGrain : Grain, ISnackMachineStatsBySnackGrain
{
    private readonly IPersistentState<SnackMachineStats> _stats;
    private readonly DomainDbContext _dbContext;
    private readonly ILogger<SnackMachineStatsBySnackGrain> _logger;

    /// <inheritdoc />
    public SnackMachineStatsBySnackGrain([PersistentState(nameof(SnackMachineStats), Constants.GrainStorageName1)] IPersistentState<SnackMachineStats> stats, DomainDbContext dbContext, ILogger<SnackMachineStatsBySnackGrain> logger)
    {
        _stats = Guard.Against.Null(stats, nameof(stats));
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
        await UpdateStatsAsync();
    }

    /// <inheritdoc />
    public Task<SnackMachineStats> GetStateAsync()
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
        _logger.LogInformation("Incremented count of machines that have this snack to {Count}", _stats.State.Count);
        return _stats.WriteStateAsync();
    }

    public Task DecrementCountAsync(int number)
    {
        _stats.State.Count -= number;
        _logger.LogInformation("Decremented count of machines that have this snack to {Count}", _stats.State.Count);
        return _stats.WriteStateAsync();
    }

    private async Task UpdateStatsAsync()
    {
        try
        {
            var id = this.GetPrimaryKey();
            var countInDb = await _dbContext.SnackMachines.CountAsync(sm => sm.IsDeleted == false && sm.Slots.Any(sl => sl.SnackPile != null && sl.SnackPile.SnackId == id));
            if (_stats.State.Count != countInDb)
            {
                _logger.LogInformation("Updated count of machines that have this snack from {OldCount} to {NewCount}", _stats.State.Count, countInDb);
                _stats.State.Count = countInDb;
                await _stats.WriteStateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update stats for snack {SnackId}", this.GetPrimaryKey());
        }
    }
}
