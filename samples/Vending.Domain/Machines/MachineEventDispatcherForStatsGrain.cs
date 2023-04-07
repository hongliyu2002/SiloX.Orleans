using Fluxera.Guards;
using Microsoft.Extensions.Logging;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;
using Vending.Domain.Abstractions.Snacks;

namespace Vending.Domain.Machines;

[ImplicitStreamSubscription(Constants.MachinesBroadcastNamespace)]
public class MachineEventDispatcherForStatsGrain : BroadcastSubscriberGrainWithStringKey<MachineEvent, MachineErrorEvent>, IMachineEventDispatcherForStatsGrain
{
    private readonly ILogger<MachineEventDispatcherForStatsGrain> _logger;

    /// <inheritdoc />
    public MachineEventDispatcherForStatsGrain(ILogger<MachineEventDispatcherForStatsGrain> logger)
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
                return DispatchEventAsync(machineEvent);
            case MachineRemovedEvent machineEvent:
                return DispatchEventAsync(machineEvent);
            case MachineLoadedEvent machineEvent:
                return DispatchEventAsync(machineEvent);
            case MachineUnloadedEvent machineEvent:
                return DispatchEventAsync(machineEvent);
            default:
                return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    protected override Task HandLeErrorEventAsync(MachineErrorEvent errorEvent)
    {
        _logger.LogWarning($"MachineErrorEvent received: {string.Join(';', errorEvent.Reasons)}");
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

    private async Task DispatchEventAsync(MachineInitializedEvent machineEvent)
    {
        try
        {
            var traceId = machineEvent.TraceId;
            var operatedAt = DateTimeOffset.UtcNow;
            var operatedBy = $"System/{GetType().Name}";
            // Update SnackStatsOfMachineGrain
            var snackIds = machineEvent.Slots.Where(sl => sl.SnackPile != null).Select(x => x.SnackPile!.SnackId).Distinct().ToArray();
            var tasks = snackIds.Select(snackId => GrainFactory.GetGrain<ISnackStatsOfMachineGrain>(snackId)).Select(statsGrain => statsGrain.IncrementCountAsync(new SnackIncrementMachineCountCommand(1, traceId, operatedAt, operatedBy)));
            var results = await Task.WhenAll(tasks);
            _logger.LogInformation("Dispatch MachineInitializedEvent: {MachineId} is dispatched. With success： {SuccessCount} failed： {FailedCount}", this.GetPrimaryKey(), results.Count(r => r.IsSuccess), results.Count(r => r.IsFailed));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dispatch MachineInitializedEvent: Exception is occurred when dispatching.");
        }
    }

    private async Task DispatchEventAsync(MachineRemovedEvent machineEvent)
    {
        try
        {
            var traceId = machineEvent.TraceId;
            var operatedAt = DateTimeOffset.UtcNow;
            var operatedBy = $"System/{GetType().Name}";
            // Update SnackStatsOfMachineGrain
            var snackIds = machineEvent.Slots.Where(sl => sl.SnackPile != null).Select(sl => sl.SnackPile!.SnackId).Distinct().ToArray();
            var tasks = snackIds.Select(snackId => GrainFactory.GetGrain<ISnackStatsOfMachineGrain>(snackId)).Select(machineStatsGrain => machineStatsGrain.DecrementCountAsync(new SnackDecrementMachineCountCommand(1, traceId, operatedAt, operatedBy)));
            var results = await Task.WhenAll(tasks);
            _logger.LogInformation("Dispatch MachineRemovedEvent: {MachineId} is dispatched. With success： {SuccessCount} failed： {FailedCount}", this.GetPrimaryKey(), results.Count(r => r.IsSuccess), results.Count(r => r.IsFailed));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dispatch MachineRemovedEvent: Exception is occurred when dispatching.");
        }
    }

    private async Task DispatchEventAsync(MachineLoadedEvent machineEvent)
    {
        try
        {
            var traceId = machineEvent.TraceId;
            var operatedAt = DateTimeOffset.UtcNow;
            var operatedBy = $"System/{GetType().Name}";
            // Update SnackStatsOfMachineGrain
            if (machineEvent.Slot is { SnackPile: { } })
            {
                var machineStatsGrain = GrainFactory.GetGrain<ISnackStatsOfMachineGrain>(machineEvent.Slot.SnackPile.SnackId);
                var result = await machineStatsGrain.IncrementCountAsync(new SnackIncrementMachineCountCommand(1, traceId, operatedAt, operatedBy));
                _logger.LogInformation("Dispatch MachineLoadedEvent: {MachineId} is dispatched. With success {IsSuccess}", this.GetPrimaryKey(), result.IsSuccess);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dispatch MachineLoadedEvent: Exception is occurred when dispatching.");
        }
    }

    private async Task DispatchEventAsync(MachineUnloadedEvent machineEvent)
    {
        try
        {
            var traceId = machineEvent.TraceId;
            var operatedAt = DateTimeOffset.UtcNow;
            var operatedBy = $"System/{GetType().Name}";
            // Update SnackStatsOfMachineGrain
            if (machineEvent.Slot is { SnackPile: { } })
            {
                var machineStatsGrain = GrainFactory.GetGrain<ISnackStatsOfMachineGrain>(machineEvent.Slot.SnackPile.SnackId);
                var result = await machineStatsGrain.DecrementCountAsync(new SnackDecrementMachineCountCommand(1, traceId, operatedAt, operatedBy));
                _logger.LogInformation("Dispatch MachineUnloadedEvent: {MachineId} is dispatched. With success {IsSuccess}", this.GetPrimaryKey(), result.IsSuccess);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dispatch MachineUnloadedEvent: Exception is occurred when dispatching.");
        }
    }
}
