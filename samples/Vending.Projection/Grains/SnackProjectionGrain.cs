using Fluxera.Guards;
using Microsoft.Extensions.Logging;
using SiloX.Domain.Abstractions;
using Vending.Domain;
using Vending.Domain.Abstractions.Events;

namespace Vending.Projection.Grains;

[ImplicitStreamSubscription(Constants.SnacksNamespace)]
public class SnackProjectionGrain : EventSubscriberGrain<SnackEvent, SnackErrorEvent>
{
    private readonly ILogger<SnackProjectionGrain> _logger;

    /// <inheritdoc />
    public SnackProjectionGrain(ILogger<SnackProjectionGrain> logger)
        : base(Constants.StreamProviderName1)
    {
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    protected override string GetStreamNamespace()
    {
        return Constants.SnacksNamespace;
    }

    /// <inheritdoc />
    protected override Task HandLeEventAsync(SnackEvent domainEvent)
    {
        switch (domainEvent)
        {
            case SnackInitializedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case SnackRemovedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case SnackNameChangedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case SnackPictureUrlChangedEvent snackEvt:
                return ApplyEventAsync(snackEvt);
            default:
                return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    protected override Task HandLeErrorEventAsync(SnackErrorEvent errorEvent)
    {
        return null;
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
        _logger.LogInformation($"Stream {Constants.SnacksNamespace} is completed.");
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(SnackInitializedEvent snackEvent)
    {
        return null;
    }

    private Task ApplyEventAsync(SnackRemovedEvent snackEvent)
    {
        return null;
    }

    private Task ApplyEventAsync(SnackNameChangedEvent snackEvent)
    {
        return null;
    }

    private Task ApplyEventAsync(SnackPictureUrlChangedEvent snackEvent)
    {
        return null;
    }
}
