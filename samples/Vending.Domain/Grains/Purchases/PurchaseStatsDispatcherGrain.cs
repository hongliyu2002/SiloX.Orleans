using Fluxera.Guards;
using Microsoft.Extensions.Logging;
using Orleans.FluentResults;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Commands;
using Vending.Domain.Abstractions.Events;
using Vending.Domain.Abstractions.Grains;

namespace Vending.Domain.Grains;

[ImplicitStreamSubscription(Constants.PurchasesBroadcastNamespace)]
public class PurchaseStatsDispatcherGrain : BroadcastSubscriberGrainWithStringKey<PurchaseEvent, PurchaseErrorEvent>, IPurchaseStatsDispatcherGrain
{
    private readonly ILogger<PurchaseStatsDispatcherGrain> _logger;

    /// <inheritdoc />
    public PurchaseStatsDispatcherGrain(ILogger<PurchaseStatsDispatcherGrain> logger) : base(Constants.StreamProviderName)
    {
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    protected override string GetBroadcastStreamNamespace()
    {
        return Constants.PurchasesBroadcastNamespace;
    }

    /// <inheritdoc />
    protected override Task HandLeEventAsync(PurchaseEvent domainEvent)
    {
        switch (domainEvent)
        {
            case PurchaseInitializedEvent purchaseEvent:
                return DispatchEventAsync(purchaseEvent);
            default:
                return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    protected override Task HandLeErrorEventAsync(PurchaseErrorEvent errorEvent)
    {
        _logger.LogWarning($"PurchaseErrorEvent received: {string.Join(';', errorEvent.Reasons)}");
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
        _logger.LogInformation($"Broadcast stream {Constants.PurchasesBroadcastNamespace} is completed.");
        return Task.CompletedTask;
    }

    private async Task DispatchEventAsync(PurchaseInitializedEvent purchaseEvent)
    {
        try
        {
            var traceId = Guid.NewGuid();
            var operatedAt = DateTimeOffset.UtcNow;
            var operatedBy = $"System/{GetType().Name}";
            var tasks = new List<Task<Result>>(4);
            // Update SnackMachinePurchaseStatsGrain
            var machineGrain = GrainFactory.GetGrain<ISnackMachinePurchaseStatsGrain>(purchaseEvent.MachineId);
            tasks.Add(machineGrain.IncrementCountAsync(new SnackMachineIncrementBoughtCountCommand(1, traceId, operatedAt, operatedBy)));
            tasks.Add(machineGrain.IncrementAmountAsync(new SnackMachineIncrementBoughtAmountCommand(purchaseEvent.BoughtPrice, traceId, operatedAt, operatedBy)));
            // Update SnackPurchaseStatsGrain
            var snackGrain = GrainFactory.GetGrain<ISnackPurchaseStatsGrain>(purchaseEvent.SnackId);
            tasks.Add(snackGrain.IncrementCountAsync(new SnackIncrementBoughtCountCommand(1, traceId, operatedAt, operatedBy)));
            tasks.Add(snackGrain.IncrementAmountAsync(new SnackIncrementBoughtAmountCommand(purchaseEvent.BoughtPrice, traceId, operatedAt, operatedBy)));
            var results = await Task.WhenAll(tasks);
            _logger.LogInformation("Dispatch PurchaseInitializedEvent: {PurchaseId} is dispatched. With success： {SuccessCount} failed： {FailedCount}", this.GetPrimaryKey(), results.Count(r => r.IsSuccess), results.Count(r => r.IsFailed));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dispatch PurchaseInitializedEvent: Exception is occurred when dispatching.");
        }
    }
}
