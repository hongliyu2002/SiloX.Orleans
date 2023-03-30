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
///     Grain implementation class SnackMachineStatsBySnackGrain.
/// </summary>
[StorageProvider(ProviderName = Constants.GrainStorageName)]
public class SnackMachineStatsBySnackGrain : StatefulGrainWithGuidKey<SnackMachineStats, SnackEvent, SnackErrorEvent>, ISnackMachineStatsBySnackGrain
{
    private readonly DomainDbContext _dbContext;
    private readonly ILogger<SnackMachineStatsBySnackGrain> _logger;

    /// <inheritdoc />
    public SnackMachineStatsBySnackGrain(DomainDbContext dbContext, ILogger<SnackMachineStatsBySnackGrain> logger) : base(Constants.StreamProviderName)
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

    /// <inheritdoc />
    public Task IncrementCountAsync(int number)
    {
        State.Count += number;
        _logger.LogInformation("Incremented count of machines that have this snack to {Count}", State.Count);
        return WriteStateAsync();
    }

    public Task DecrementCountAsync(int number)
    {
        State.Count -= number;
        _logger.LogInformation("Decremented count of machines that have this snack to {Count}", State.Count);
        return WriteStateAsync();
    }

    #region Update From DB

    private async Task UpdateCountAsync()
    {
        var snackId = this.GetPrimaryKey();
        try
        {
            var count = await _dbContext.SnackMachines.CountAsync(sm => sm.IsDeleted == false && sm.Slots.Any(sl => sl.SnackPile != null && sl.SnackPile.SnackId == snackId));
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
