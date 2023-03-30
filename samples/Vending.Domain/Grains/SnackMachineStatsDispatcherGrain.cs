using Fluxera.Guards;
using Microsoft.Extensions.Logging;
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
            case SnackMachineSnackBoughtEvent machineEvent:
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
            var snackSnackMachineStatsGrain = GrainFactory.GetGrain<ISnackSnackMachineStatsGrain>(machineEvent.MachineId);
            await snackSnackMachineStatsGrain.IncrementCountAsync(new SnackIncrementMachineCountCommand(1, traceId, operatedAt, operatedBy));
            // _logger.LogInformation("Dispatch SnackMachineInitializedEvent: {SnackMachineId} is dispatched.", this.GetPrimaryKeyString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dispatch SnackMachineInitializedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
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
            var snackSnackMachineStatsGrain = GrainFactory.GetGrain<ISnackSnackMachineStatsGrain>(machineEvent.MachineId);
            await snackSnackMachineStatsGrain.IncrementCountAsync(new SnackIncrementMachineCountCommand(1, traceId, operatedAt, operatedBy));
            // _logger.LogInformation("Dispatch SnackMachineInitializedEvent: {SnackMachineId} is dispatched.", this.GetPrimaryKeyString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dispatch SnackMachineInitializedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
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
            var snackSnackMachineStatsGrain = GrainFactory.GetGrain<ISnackSnackMachineStatsGrain>(machineEvent.MachineId);
            await snackSnackMachineStatsGrain.IncrementCountAsync(new SnackIncrementMachineCountCommand(1, traceId, operatedAt, operatedBy));
            // _logger.LogInformation("Dispatch SnackMachineInitializedEvent: {SnackMachineId} is dispatched.", this.GetPrimaryKeyString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dispatch SnackMachineInitializedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
        }
    }
    
    private async Task DispatchEventAsync(SnackMachineSnackBoughtEvent machineEvent)
    {
        try
        {
            var traceId = Guid.NewGuid();
            var operatedAt = DateTimeOffset.UtcNow;
            var operatedBy = $"System/{GetType().Name}";
            // Update SnackSnackMachineStatsGrain
            var snackSnackMachineStatsGrain = GrainFactory.GetGrain<ISnackSnackMachineStatsGrain>(machineEvent.MachineId);
            await snackSnackMachineStatsGrain.IncrementCountAsync(new SnackIncrementMachineCountCommand(1, traceId, operatedAt, operatedBy));
            // _logger.LogInformation("Dispatch SnackMachineInitializedEvent: {SnackMachineId} is dispatched.", this.GetPrimaryKeyString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dispatch SnackMachineInitializedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
        }
    }
}
