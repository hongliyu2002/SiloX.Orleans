using Fluxera.Guards;
using Microsoft.Extensions.Logging;
using Orleans.FluentResults;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;
using Vending.Domain.Abstractions.Snacks;

namespace Vending.Domain.Machines;

[ImplicitStreamSubscription(Constants.MachinesBroadcastNamespace)]
public class MachineEventHubGrain : BroadcastSubscriberGrainWithStringKey<MachineEvent, MachineErrorEvent>, IMachineEventHubGrain
{
    private readonly ILogger<MachineEventHubGrain> _logger;

    /// <inheritdoc />
    public MachineEventHubGrain(ILogger<MachineEventHubGrain> logger)
        : base(Constants.StreamProviderName)
    {
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    protected override string GetBroadcastStreamNamespace()
    {
        return Constants.MachinesBroadcastNamespace;
    }

    /// <inheritdoc />
    protected override Task HandLeEventAsync(MachineEvent domainEvent)
    {
        switch (domainEvent)
        {
            case MachineInitializedEvent machineEvent:
                return DispatchTasksAsync(machineEvent);
            case MachineRemovedEvent machineEvent:
                return DispatchTasksAsync(machineEvent);
            // case MachineMoneyLoadedEvent machineEvent:
            //     return DispatchTasksAsync(machineEvent);
            // case MachineMoneyUnloadedEvent machineEvent:
            //     return DispatchTasksAsync(machineEvent);
            // case MachineMoneyInsertedEvent machineEvent:
            //     return DispatchTasksAsync(machineEvent);
            // case MachineMoneyReturnedEvent machineEvent:
            //     return DispatchTasksAsync(machineEvent);
            case MachineSnacksLoadedEvent machineEvent:
                return DispatchTasksAsync(machineEvent);
            case MachineSnacksUnloadedEvent machineEvent:
                return DispatchTasksAsync(machineEvent);
            case MachineSnackBoughtEvent machineEvent:
                return DispatchTasksAsync(machineEvent);
            default:
                return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    protected override Task HandLeErrorEventAsync(MachineErrorEvent errorEvent)
    {
        _logger.LogWarning("MachineErrorEvent received: {Reasons}", string.Join(';', errorEvent.Reasons));
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task HandleExceptionAsync(Exception exception)
    {
        _logger.LogError(exception, "Exception is {Message}", exception.Message);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task HandleCompleteAsync()
    {
        _logger.LogInformation($"Broadcast stream {Constants.MachinesBroadcastNamespace} is completed.");
        return Task.CompletedTask;
    }

    private async Task DispatchTasksAsync(MachineInitializedEvent machineEvent)
    {
        try
        {
            // Update SnackStatsOfMachinesGrain
            var snackIds = machineEvent.Slots.Where(ms => ms.SnackPile != null).Select(x => x.SnackPile!.SnackId).Distinct().ToArray();
            var snackStatsGrains = snackIds.Select(snackId => GrainFactory.GetGrain<ISnackStatsOfMachinesGrain>(snackId));
            var tasks = snackStatsGrains.SelectMany(snackStatsGrain => new List<Task<Result>>(3)
                                                                       {
                                                                           snackStatsGrain.UpdateMachineCountAsync(-1),
                                                                           snackStatsGrain.UpdateTotalQuantityAsync(-1),
                                                                           snackStatsGrain.UpdateTotalAmountAsync(-1)
                                                                       });
            var results = await Task.WhenAll(tasks);
            _logger.LogInformation("Handle MachineInitializedEvent: Machine {MachineId} tasks is dispatched. With success： {SuccessCount} failed： {FailedCount}", this.GetPrimaryKey(), results.Count(r => r.IsSuccess), results.Count(r => r.IsFailed));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle MachineInitializedEvent: Exception is occurred when dispatching");
        }
    }

    private async Task DispatchTasksAsync(MachineRemovedEvent machineEvent)
    {
        try
        {
            // Update SnackStatsOfMachinesGrain
            var snackIds = machineEvent.Slots.Where(ms => ms.SnackPile != null).Select(x => x.SnackPile!.SnackId).Distinct().ToArray();
            var snackStatsGrains = snackIds.Select(snackId => GrainFactory.GetGrain<ISnackStatsOfMachinesGrain>(snackId));
            var tasks = snackStatsGrains.SelectMany(snackStatsGrain => new List<Task<Result>>(3)
                                                                       {
                                                                           snackStatsGrain.UpdateMachineCountAsync(-1),
                                                                           snackStatsGrain.UpdateTotalQuantityAsync(-1),
                                                                           snackStatsGrain.UpdateTotalAmountAsync(-1)
                                                                       });
            var results = await Task.WhenAll(tasks);
            _logger.LogInformation("Handle MachineRemovedEvent: Machine {MachineId} tasks is dispatched. With success： {SuccessCount} failed： {FailedCount}", this.GetPrimaryKey(), results.Count(r => r.IsSuccess), results.Count(r => r.IsFailed));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle MachineRemovedEvent: Exception is occurred when dispatching");
        }
    }

    private async Task DispatchTasksAsync(MachineSnacksLoadedEvent machineEvent)
    {
        try
        {
            // Update SnackStatsOfMachinesGrain
            if (machineEvent.Slot is { SnackPile: not null })
            {
                var snackStatsGrain = GrainFactory.GetGrain<ISnackStatsOfMachinesGrain>(machineEvent.Slot.SnackPile.SnackId);
                var tasks = new List<Task<Result>>(2) { snackStatsGrain.UpdateTotalQuantityAsync(-1), snackStatsGrain.UpdateTotalAmountAsync(-1) };
                var results = await Task.WhenAll(tasks);
                _logger.LogInformation("Handle MachineSnacksLoadedEvent: Machine {MachineId} tasks is dispatched. With success： {SuccessCount} failed： {FailedCount}", this.GetPrimaryKey(), results.Count(r => r.IsSuccess), results.Count(r => r.IsFailed));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle MachineSnacksLoadedEvent: Exception is occurred when dispatching");
        }
    }

    private async Task DispatchTasksAsync(MachineSnacksUnloadedEvent machineEvent)
    {
        try
        {
            // Update SnackStatsOfMachinesGrain
            if (machineEvent.Slot is { SnackPile: not null })
            {
                var snackStatsGrain = GrainFactory.GetGrain<ISnackStatsOfMachinesGrain>(machineEvent.Slot.SnackPile.SnackId);
                var tasks = new List<Task<Result>>(2) { snackStatsGrain.UpdateTotalQuantityAsync(-1), snackStatsGrain.UpdateTotalAmountAsync(-1) };
                var results = await Task.WhenAll(tasks);
                _logger.LogInformation("Handle MachineSnacksUnloadedEvent: Machine {MachineId} tasks is dispatched. With success： {SuccessCount} failed： {FailedCount}", this.GetPrimaryKey(), results.Count(r => r.IsSuccess), results.Count(r => r.IsFailed));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle MachineSnacksUnloadedEvent: Exception is occurred when dispatching");
        }
    }
    
    private async Task DispatchTasksAsync(MachineSnackBoughtEvent machineEvent)
    {
        try
        {
            // Update SnackStatsOfMachinesGrain
            if (machineEvent.Slot is { SnackPile: not null })
            {
                var snackStatsGrain = GrainFactory.GetGrain<ISnackStatsOfMachinesGrain>(machineEvent.Slot.SnackPile.SnackId);
                var tasks = new List<Task<Result>>(2) { snackStatsGrain.UpdateTotalQuantityAsync(-1), snackStatsGrain.UpdateTotalAmountAsync(-1) };
                var results = await Task.WhenAll(tasks);
                _logger.LogInformation("Handle MachineSnackBoughtEvent: Machine {MachineId} tasks is dispatched. With success： {SuccessCount} failed： {FailedCount}", this.GetPrimaryKey(), results.Count(r => r.IsSuccess), results.Count(r => r.IsFailed));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle MachineSnackBoughtEvent: Exception is occurred when dispatching");
        }
    }
}
