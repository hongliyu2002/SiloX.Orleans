using Fluxera.Guards;
using Microsoft.Extensions.Logging;
using Orleans.FluentResults;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Commands;
using Vending.Domain.Abstractions.Events;
using Vending.Domain.Abstractions.Grains;

namespace Vending.Domain.Grains;

[ImplicitStreamSubscription(Constants.SnackMachinesBroadcastNamespace)]
public class SnackMachineStatsDispatcherGrain : BroadcastSubscriberGrainWithStringKey<SnackMachineEvent, SnackMachineErrorEvent>, ISnackMachineStatsDispatcherGrain
{
    private readonly ILogger<SnackMachineStatsDispatcherGrain> _logger;

    /// <inheritdoc />
    public SnackMachineStatsDispatcherGrain(ILogger<SnackMachineStatsDispatcherGrain> logger) : base(Constants.StreamProviderName)
    {
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    protected override string GetBroadcastStreamNamespace()
    {
        return Constants.SnackMachinesBroadcastNamespace;
    }

    /// <inheritdoc />
    protected override Task HandLeEventAsync(SnackMachineEvent domainEvent)
    {
        switch (domainEvent)
        {
            case SnackMachineInitializedEvent machineEvent:
                return DispatchEventAsync(machineEvent);
            case SnackMachineRemovedEvent machineEvent:
                return DispatchEventAsync(machineEvent);
            case SnackMachineSnacksLoadedEvent machineEvent:
                return DispatchEventAsync(machineEvent);
            case SnackMachineSnacksUnloadedEvent machineEvent:
                return DispatchEventAsync(machineEvent);
            default:
                return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    protected override Task HandLeErrorEventAsync(SnackMachineErrorEvent errorEvent)
    {
        _logger.LogWarning($"SnackMachineErrorEvent received: {string.Join(';', errorEvent.Reasons)}");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task HandleExceptionAsync(Exception exception)
    {
        _logger.LogError(exception, exception.Message);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task HandleCompleteAsync()
    {
        _logger.LogInformation($"Broadcast stream {Constants.SnackMachinesBroadcastNamespace} is completed.");
        return Task.CompletedTask;
    }

    private async Task DispatchEventAsync(SnackMachineInitializedEvent machineEvent)
    {
        try
        {
            var traceId = Guid.NewGuid();
            var operatedAt = DateTimeOffset.UtcNow;
            var operatedBy = $"System/{GetType().Name}";
            // Update SnackSnackMachineStatsGrain
            var snackIds = machineEvent.Slots.Where(sl => sl.SnackPile != null).Select(x => x.SnackPile!.SnackId).Distinct().ToArray();
            var tasks = snackIds.Select(snackId => GrainFactory.GetGrain<ISnackSnackMachineStatsGrain>(snackId)).Select(statsGrain => statsGrain.IncrementCountAsync(new SnackIncrementMachineCountCommand(1, traceId, operatedAt, operatedBy)));
            var results = await Task.WhenAll(tasks);
            _logger.LogInformation("Dispatch SnackMachineInitializedEvent: {SnackMachineId} is dispatched. With success： {SuccessCount} failed： {FailedCount}", this.GetPrimaryKey(), results.Count(r => r.IsSuccess), results.Count(r => r.IsFailed));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dispatch SnackMachineInitializedEvent: Exception is occurred when dispatching.");
        }
    }

    private async Task DispatchEventAsync(SnackMachineRemovedEvent machineEvent)
    {
        try
        {
            var traceId = Guid.NewGuid();
            var operatedAt = DateTimeOffset.UtcNow;
            var operatedBy = $"System/{GetType().Name}";
            // Update SnackSnackMachineStatsGrain
            var snackIds = machineEvent.Slots.Where(sl => sl.SnackPile != null).Select(x => x.SnackPile!.SnackId).Distinct().ToArray();
            var tasks = snackIds.Select(snackId => GrainFactory.GetGrain<ISnackSnackMachineStatsGrain>(snackId)).Select(statsGrain => statsGrain.DecrementCountAsync(new SnackDecrementMachineCountCommand(1, traceId, operatedAt, operatedBy)));
            var results = await Task.WhenAll(tasks);
            _logger.LogInformation("Dispatch SnackMachineRemovedEvent: {SnackMachineId} is dispatched. With success： {SuccessCount} failed： {FailedCount}", this.GetPrimaryKey(), results.Count(r => r.IsSuccess), results.Count(r => r.IsFailed));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dispatch SnackMachineRemovedEvent: Exception is occurred when dispatching.");
        }
    }

    private async Task DispatchEventAsync(SnackMachineSnacksLoadedEvent machineEvent)
    {
        try
        {
            var traceId = Guid.NewGuid();
            var operatedAt = DateTimeOffset.UtcNow;
            var operatedBy = $"System/{GetType().Name}";
            // Update SnackSnackMachineStatsGrain
            if (machineEvent.Slot is { SnackPile: { } })
            {
                var statsGrain = GrainFactory.GetGrain<ISnackSnackMachineStatsGrain>(machineEvent.Slot.SnackPile.SnackId);
                var result = await statsGrain.IncrementCountAsync(new SnackIncrementMachineCountCommand(1, traceId, operatedAt, operatedBy));
                _logger.LogInformation("Dispatch SnackMachineSnacksLoadedEvent: {SnackMachineId} is dispatched. With success {IsSuccess}", this.GetPrimaryKeyString(), result.IsSuccess);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dispatch SnackMachineSnacksLoadedEvent: Exception is occurred when dispatching.");
        }
    }

    private async Task DispatchEventAsync(SnackMachineSnacksUnloadedEvent machineEvent)
    {
        try
        {
            var traceId = Guid.NewGuid();
            var operatedAt = DateTimeOffset.UtcNow;
            var operatedBy = $"System/{GetType().Name}";
            // Update SnackSnackMachineStatsGrain
            if (machineEvent.Slot is { SnackPile: { } })
            {
                var statsGrain = GrainFactory.GetGrain<ISnackSnackMachineStatsGrain>(machineEvent.Slot.SnackPile.SnackId);
                Result result = await statsGrain.DecrementCountAsync(new SnackDecrementMachineCountCommand(1, traceId, operatedAt, operatedBy));
                _logger.LogInformation("Dispatch SnackMachineSnacksUnloadedEvent: {SnackMachineId} is dispatched. With success {IsSuccess}", this.GetPrimaryKeyString(), result.IsSuccess);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dispatch SnackMachineSnacksUnloadedEvent: Exception is occurred when dispatching.");
        }
    }
}
