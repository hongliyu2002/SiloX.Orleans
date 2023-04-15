using Fluxera.Guards;
using Microsoft.Extensions.Logging;
using Orleans.FluentResults;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;
using Vending.Domain.Abstractions.Snacks;

namespace Vending.Domain.Machines;

[ImplicitStreamSubscription(Constants.MachinesBroadcastNamespace)]
public class MachineEventHubGrain : ReceiverGrainWithGuidKey<MachineEvent, MachineErrorEvent>, IMachineEventHubGrain
{
    private readonly ILogger<MachineEventHubGrain> _logger;

    /// <inheritdoc />
    public MachineEventHubGrain(ILogger<MachineEventHubGrain> logger)
        : base(Constants.StreamProviderName)
    {
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    protected override string GetSubBroadcastStreamNamespace()
    {
        return Constants.MachinesBroadcastNamespace;
    }

    /// <inheritdoc />
    protected override Task HandLeEventAsync(MachineEvent domainEvent)
    {
        return domainEvent switch
               {
                   MachineInitializedEvent machineEvent => DispatchTasksAsync(machineEvent),
                   MachineDeletedEvent machineEvent => DispatchTasksAsync(machineEvent),
                   MachineUpdatedEvent machineEvent => DispatchTasksAsync(machineEvent),
                   MachineSlotAddedEvent machineEvent => DispatchTasksAsync(machineEvent),
                   MachineSlotRemovedEvent machineEvent => DispatchTasksAsync(machineEvent),
                   // MachineMoneyLoadedEvent machineEvent => DispatchTasksAsync(machineEvent),
                   // MachineMoneyUnloadedEvent machineEvent => DispatchTasksAsync(machineEvent),
                   // MachineMoneyInsertedEvent machineEvent => DispatchTasksAsync(machineEvent),
                   // MachineMoneyReturnedEvent machineEvent => DispatchTasksAsync(machineEvent),
                   MachineSnacksLoadedEvent machineEvent => DispatchTasksAsync(machineEvent),
                   MachineSnacksUnloadedEvent machineEvent => DispatchTasksAsync(machineEvent),
                   MachineSnackBoughtEvent machineEvent => DispatchTasksAsync(machineEvent),
                   // MachineBoughtCountUpdatedEvent machineEvent => DispatchTasksAsync(machineEvent),
                   // MachineBoughtAmountUpdatedEvent machineEvent => DispatchTasksAsync(machineEvent),
                   _ => Task.CompletedTask
               };
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

    private async Task DispatchTasksAsync(MachineDeletedEvent machineEvent)
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
            _logger.LogInformation("Handle MachineDeletedEvent: Machine {MachineId} tasks is dispatched. With success： {SuccessCount} failed： {FailedCount}", this.GetPrimaryKey(), results.Count(r => r.IsSuccess), results.Count(r => r.IsFailed));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle MachineDeletedEvent: Exception is occurred when dispatching");
        }
    }

    private async Task DispatchTasksAsync(MachineUpdatedEvent machineEvent)
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
            _logger.LogInformation("Handle MachineUpdatedEvent: Machine {MachineId} tasks is dispatched. With success： {SuccessCount} failed： {FailedCount}", this.GetPrimaryKey(), results.Count(r => r.IsSuccess), results.Count(r => r.IsFailed));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle MachineUpdatedEvent: Exception is occurred when dispatching");
        }
    }

    private async Task DispatchTasksAsync(MachineSlotAddedEvent machineEvent)
    {
        try
        {
            // Update SnackStatsOfMachinesGrain
            if (machineEvent.Slot is { SnackPile: not null })
            {
                var snackStatsGrain = GrainFactory.GetGrain<ISnackStatsOfMachinesGrain>(machineEvent.Slot.SnackPile.SnackId);
                var tasks = new List<Task<Result>>(3)
                            {
                                snackStatsGrain.UpdateMachineCountAsync(-1),
                                snackStatsGrain.UpdateTotalQuantityAsync(-1),
                                snackStatsGrain.UpdateTotalAmountAsync(-1)
                            };
                var results = await Task.WhenAll(tasks);
                _logger.LogInformation("Handle MachineSlotAddedEvent: Machine {MachineId} tasks is dispatched. With success： {SuccessCount} failed： {FailedCount}", this.GetPrimaryKey(), results.Count(r => r.IsSuccess), results.Count(r => r.IsFailed));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle MachineSlotAddedEvent: Exception is occurred when dispatching");
        }
    }

    private async Task DispatchTasksAsync(MachineSlotRemovedEvent machineEvent)
    {
        try
        {
            // Update SnackStatsOfMachinesGrain
            if (machineEvent.Slot is { SnackPile: not null })
            {
                var snackStatsGrain = GrainFactory.GetGrain<ISnackStatsOfMachinesGrain>(machineEvent.Slot.SnackPile.SnackId);
                var tasks = new List<Task<Result>>(3)
                            {
                                snackStatsGrain.UpdateMachineCountAsync(-1),
                                snackStatsGrain.UpdateTotalQuantityAsync(-1),
                                snackStatsGrain.UpdateTotalAmountAsync(-1)
                            };
                var results = await Task.WhenAll(tasks);
                _logger.LogInformation("Handle MachineSlotRemovedEvent: Machine {MachineId} tasks is dispatched. With success： {SuccessCount} failed： {FailedCount}", this.GetPrimaryKey(), results.Count(r => r.IsSuccess), results.Count(r => r.IsFailed));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle MachineSlotRemovedEvent: Exception is occurred when dispatching");
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
                var tasks = new List<Task<Result>>(3)
                            {
                                snackStatsGrain.UpdateMachineCountAsync(-1),
                                snackStatsGrain.UpdateTotalQuantityAsync(-1),
                                snackStatsGrain.UpdateTotalAmountAsync(-1)
                            };
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
                var tasks = new List<Task<Result>>(3)
                            {
                                snackStatsGrain.UpdateMachineCountAsync(-1),
                                snackStatsGrain.UpdateTotalQuantityAsync(-1),
                                snackStatsGrain.UpdateTotalAmountAsync(-1)
                            };
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
                var tasks = new List<Task<Result>>(3)
                            {
                                snackStatsGrain.UpdateMachineCountAsync(-1),
                                snackStatsGrain.UpdateTotalQuantityAsync(-1),
                                snackStatsGrain.UpdateTotalAmountAsync(-1)
                            };
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