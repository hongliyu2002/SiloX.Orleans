using Fluxera.Guards;
using Orleans.Runtime;
using Orleans.Streams;

namespace SiloX.Domain.Abstractions;

/// <summary>
///     Represents a base class for Orleans event subscribers that can subscribe to a stream of TEvent from Broadcast.
/// </summary>
public abstract class ReceiverGrainWithStringKey<TSubEvent, TSubErrorEvent> : Grain, IGrainWithStringKey
    where TSubEvent : DomainEvent
    where TSubErrorEvent : TSubEvent, IDomainErrorEvent
{
    private readonly IStreamProvider _subStreamProvider;
    private IAsyncStream<TSubEvent>? _subBroadcastStream;
    private StreamSubscriptionHandle<TSubEvent>? _subscription;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ReceiverGrainWithStringKey{TSubEvent,TSubErrorEvent}" /> class with
    ///     the specified stream provider name and stream namespace.
    /// </summary>
    /// <param name="streamProviderName">The name of the stream provider.</param>
    protected ReceiverGrainWithStringKey(string streamProviderName)
    {
        streamProviderName = Guard.Against.NullOrWhiteSpace(streamProviderName, nameof(streamProviderName));
        _subStreamProvider = this.GetStreamProvider(streamProviderName);
    }

    /// <inheritdoc />
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
        _subscription = await GetSubBroadcastStream().SubscribeAsync(HandleNextAsync, HandleExceptionAsync, HandleCompleteAsync);
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
    ///     Gets the broadcast stream namespace.
    /// </summary>
    /// <returns>The stream namespace.</returns>
    protected abstract string GetSubBroadcastStreamNamespace();

    /// <summary>
    ///     Gets the broadcast stream for the grain.
    /// </summary>
    /// <returns>The stream.</returns>
    private IAsyncStream<TSubEvent> GetSubBroadcastStream()
    {
        return _subBroadcastStream ??= _subStreamProvider.GetStream<TSubEvent>(StreamId.Create(GetSubBroadcastStreamNamespace(), string.Empty));
    }

    /// <summary>
    ///     Handles the next event in the stream.
    /// </summary>
    /// <param name="domainEvent">The next event in the stream.</param>
    /// <param name="sequenceToken">The sequence token of the event.</param>
    private Task HandleNextAsync(TSubEvent domainEvent, StreamSequenceToken sequenceToken)
    {
        if (domainEvent is TSubErrorEvent errorEvent)
        {
            return HandLeErrorEventAsync(errorEvent);
        }
        return HandLeEventAsync(domainEvent);
    }

    /// <summary>
    ///     Handles a successful domain event in the stream.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    protected abstract Task HandLeEventAsync(TSubEvent domainEvent);

    /// <summary>
    ///     Handles an error event in the stream.
    /// </summary>
    /// <param name="errorEvent">The error event.</param>
    protected abstract Task HandLeErrorEventAsync(TSubErrorEvent errorEvent);

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
