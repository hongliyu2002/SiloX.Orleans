using Orleans.Streams;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions.Events;

namespace Vending.Domain.Grains;

[ImplicitStreamSubscription(Constants.SnacksNamespace)]
public class SnackProjectionGrain : EventSubscriberGrain<SnackEvent, SnackErrorEvent>
{
    /// <inheritdoc />
    public SnackProjectionGrain()
        : base(Constants.StreamProviderName1)
    {
    }

    /// <inheritdoc />
    protected override string GetStreamNamespace()
    {
        return Constants.SnacksNamespace;
    }

    /// <inheritdoc />
    protected override Task HandLeEventAsync(SnackEvent domainEvent)
    {
        return null;
    }

    /// <inheritdoc />
    protected override Task HandLeErrorEventAsync(SnackErrorEvent errorEvent)
    {
        return null;
    }

    /// <inheritdoc />
    protected override Task HandleExceptionAsync(Exception exception)
    {
        return null;
    }

    /// <inheritdoc />
    protected override Task HandleCompleteAsync()
    {
        return null;
    }
}
