using Fluxera.Guards;
using Orleans.Runtime;
using Orleans.Streams;

namespace SiloX.Domain.Abstractions;

/// <summary>
///     Represents a base class for Orleans event subscribers that can subscribe to a stream of TEvent.
/// </summary>
public abstract class SubscriberGrain<TEvent, TErrorEvent> : Grain, IGrainWithGuidKey
    where TEvent : DomainEvent
{
    private readonly IStreamProvider _subscribeStreamProvider;
    private IAsyncStream<TEvent>? _subscribeStream;
    private StreamSubscriptionHandle<TEvent>? _subscription;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SubscriberGrain{TEvent,TErrorEvent}" /> class with the specified stream provider name and stream namespace.
    /// </summary>
    /// <param name="streamProviderName">The name of the stream provider.</param>
    protected SubscriberGrain(string streamProviderName)
    {
        streamProviderName = Guard.Against.NullOrWhiteSpace(streamProviderName, nameof(streamProviderName));
        _subscribeStreamProvider = this.GetStreamProvider(streamProviderName);
    }

    /// <inheritdoc />
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
        _subscription = await GetSubscribeStream().SubscribeAsync(HandleNextAsync, HandleExceptionAsync, HandleCompleteAsync);
    }

    /// <inheritdoc />
    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        if (_subscription != null)
        {
            await _subscription.UnsubscribeAsync();
        }
        var subscriptions = await GetSubscribeStream().GetAllSubscriptionHandles();
        if (subscriptions is { Count: > 0 })
        {
            await Task.WhenAll(subscriptions.Select(subscription => subscription.UnsubscribeAsync()));
        }
        await base.OnDeactivateAsync(reason, cancellationToken);
    }

    /// <summary>
    ///     Gets the stream namespace.
    /// </summary>
    /// <returns>The stream namespace.</returns>
    protected abstract string GetSubscribeStreamNamespace();

    /// <summary>
    ///     Gets the stream for the grain.
    /// </summary>
    /// <returns>The stream.</returns>
    private IAsyncStream<TEvent> GetSubscribeStream()
    {
        return _subscribeStream ??= _subscribeStreamProvider.GetStream<TEvent>(StreamId.Create(GetSubscribeStreamNamespace(), this.GetPrimaryKey()));
    }

    /// <summary>
    ///     Handles the next event in the stream.
    /// </summary>
    /// <param name="domainEvent">The next event in the stream.</param>
    /// <param name="sequenceToken">The sequence token of the event.</param>
    private Task HandleNextAsync(TEvent domainEvent, StreamSequenceToken sequenceToken)
    {
        if (domainEvent is TErrorEvent errorEvent)
        {
            return HandLeErrorEventAsync(errorEvent);
        }
        return HandLeEventAsync(domainEvent);
    }

    /// <summary>
    ///     Handles a successful domain event in the stream.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    protected abstract Task HandLeEventAsync(TEvent domainEvent);

    /// <summary>
    ///     Handles an error event in the stream.
    /// </summary>
    /// <param name="errorEvent">The error event.</param>
    protected abstract Task HandLeErrorEventAsync(TErrorEvent errorEvent);

    /// <summary>
    ///     Handles an exception that occurred while processing the stream.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    protected abstract Task HandleExceptionAsync(Exception exception);

    /// <summary>
    ///     Handles the completion of the stream.
    /// </summary>
    protected abstract Task HandleCompleteAsync();
}
