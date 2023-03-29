using Fluxera.Guards;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Grains;
using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Grains;

public class SnackMachineStatsBySnackGrain : Grain, ISnackMachineStatsBySnackGrain
{
    private readonly IPersistentState<SnackMachineStatsBySnack> _stats;
    private readonly ILogger<SnackMachineStatsBySnackGrain> _logger;

    /// <inheritdoc />
    public SnackMachineStatsBySnackGrain([PersistentState(nameof(SnackMachineStatsBySnack), Constants.GrainStorageName1)] IPersistentState<SnackMachineStatsBySnack> stats, ILogger<SnackMachineStatsBySnackGrain> logger)
    {
        _stats = Guard.Against.Null(stats, nameof(stats));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    public Task<SnackMachineStatsBySnack> GetStateAsync()
    {
        return Task.FromResult(_stats.State);
    }

    /// <inheritdoc />
    public Task<int> GetMachineCountAsync()
    {
        return Task.FromResult(_stats.State.Count);
    }

    /// <inheritdoc />
    public Task IncrementCountAsync(int numOfMachines)
    {
        _stats.State.Count += numOfMachines;
        _logger.LogInformation("Incremented count of machines that have this snack to {Count}", _stats.State.Count);
        return _stats.WriteStateAsync();
    }

    
    public Task DecrementCountAsync(int numOfMachines)
    {
        _stats.State.Count -= numOfMachines;
        _logger.LogInformation("Decremented count of machines that have this snack to {Count}", _stats.State.Count);
        return _stats.WriteStateAsync();
    }
}
