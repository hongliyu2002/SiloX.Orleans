using Fluxera.Guards;
using Microsoft.Extensions.Logging;
using Orleans.FluentResults;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;
using Vending.Domain.Abstractions.Purchases;
using Vending.Domain.Abstractions.Snacks;

namespace Vending.Domain.Purchases;

[ImplicitStreamSubscription(Constants.PurchasesBroadcastNamespace)]
public class PurchaseEventHubGrain : ReceiverGrainWithStringKey<PurchaseEvent, PurchaseErrorEvent>, IPurchaseEventHubGrain
{
    private readonly ILogger<PurchaseEventHubGrain> _logger;

    /// <inheritdoc />
    public PurchaseEventHubGrain(ILogger<PurchaseEventHubGrain> logger)
        : base(Constants.StreamProviderName)
    {
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    protected override string GetSubBroadcastStreamNamespace()
    {
        return Constants.PurchasesBroadcastNamespace;
    }

    /// <inheritdoc />
    protected override Task HandLeEventAsync(PurchaseEvent domainEvent)
    {
        switch (domainEvent)
        {
            case PurchaseInitializedEvent purchaseEvent:
                return DispatchTasksAsync(purchaseEvent);
            default:
                return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    protected override Task HandLeErrorEventAsync(PurchaseErrorEvent errorEvent)
    {
        _logger.LogWarning("PurchaseErrorEvent received: {Reasons}", string.Join(';', errorEvent.Reasons));
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
        _logger.LogInformation($"Broadcast stream {Constants.PurchasesBroadcastNamespace} is completed.");
        return Task.CompletedTask;
    }

    private async Task DispatchTasksAsync(PurchaseInitializedEvent purchaseEvent)
    {
        try
        {
            var tasks = new List<Task<Result>>(4);
            // Update MachineStatsOfPurchasesGrain
            var machineStatsGrain = GrainFactory.GetGrain<IMachineStatsOfPurchasesGrain>(purchaseEvent.MachineId);
            tasks.Add(machineStatsGrain.UpdateBoughtCountAsync(-1));
            tasks.Add(machineStatsGrain.UpdateBoughtAmountAsync(-1));
            // Update SnackStatsOfPurchasesGrain
            var snackStatsGrain = GrainFactory.GetGrain<ISnackStatsOfPurchasesGrain>(purchaseEvent.SnackId);
            tasks.Add(snackStatsGrain.UpdateBoughtCountAsync(-1));
            tasks.Add(snackStatsGrain.UpdateBoughtAmountAsync(-1));
            var results = await Task.WhenAll(tasks);
            _logger.LogInformation("Dispatch PurchaseInitializedEvent: Purchase {PurchaseId} tasks is dispatched. With success： {SuccessCount} failed： {FailedCount}", this.GetPrimaryKey(), results.Count(r => r.IsSuccess), results.Count(r => r.IsFailed));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dispatch PurchaseInitializedEvent: Exception is occurred when dispatching");
        }
    }
}
