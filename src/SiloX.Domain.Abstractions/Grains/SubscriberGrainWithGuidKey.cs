using Fluxera.Guards;
using Orleans.Runtime;
using Orleans.Streams;

namespace SiloX.Domain.Abstractions;

/// <summary>
///     Represents a base class for Orleans event subscribers that can subscribe to a stream of TEvent.
/// </summary>
public abstract class SubscriberGrainWithGuidKey<TEvent, TErrorEvent> : Grain, IGrainWithGuidKey
    where TEvent : DomainEvent
    where TErrorEvent : TEvent, IDomainErrorEvent
{
    private readonly IStreamProvider _streamProvider;
    private IAsyncStream<TEvent>? _stream;
    private StreamSubscriptionHandle<TEvent>? _subscription;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SubscriberGrainWithGuidKey{TEvent,TErrorEvent}" /> class with
    ///     the specified stream provider name and stream namespace.
    /// </summary>
    /// <param name="streamProviderName">The name of the stream provider.</param>
    protected SubscriberGrainWithGuidKey(string streamProviderName)
    {
        streamProviderName = Guard.Against.NullOrWhiteSpace(streamProviderName, nameof(streamProviderName));
        _streamProvider = this.GetStreamProvider(streamProviderName);
    }

    /// <inheritdoc />
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
        _subscription = await GetStream().SubscribeAsync(HandleNextAsync, HandleExceptionAsync, HandleCompleteAsync);
    }

    /// <inheritdoc />
    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        if (_subscription != null)
        {
            await _subscription.UnsubscribeAsync();
        }
        await base.OnDeactivateAsync(reason, cancellationToken);
    }

    /// <summary>
    ///     Gets the stream namespace.
    /// </summary>
    /// <returns>The stream namespace.</returns>
    protected abstract string GetStreamNamespace();

    /// <summary>
    ///     Gets the stream for the grain.
    /// </summary>
    /// <returns>The stream.</returns>
    private IAsyncStream<TEvent> GetStream()
    {
        return _stream ??= _streamProvider.GetStream<TEvent>(StreamId.Create(GetStreamNamespace(), this.GetPrimaryKey()));
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
