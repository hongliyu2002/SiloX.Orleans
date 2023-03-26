using Fluxera.Guards;
using Orleans.Runtime;
using Orleans.Streams;

namespace SiloX.Domain.Abstractions;

/// <summary>
///     Represents a base class for Orleans event subscribers that can subscribe to a stream of <see cref="TEvent" />.
/// </summary>
public abstract class EventSubscriberGrain<TEvent> : Grain, IGrainWithGuidKey
    where TEvent : DomainEvent
{
    private readonly IStreamProvider _streamProvider;
    private IAsyncStream<TEvent>? _stream;
    private StreamSubscriptionHandle<TEvent>? _subscription;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EventSubscriberGrain{TEvent}" /> class with the specified stream provider name and stream namespace.
    /// </summary>
    /// <param name="streamProviderName">The name of the stream provider.</param>
    protected EventSubscriberGrain(string streamProviderName)
    {
        streamProviderName = Guard.Against.NullOrWhiteSpace(streamProviderName, nameof(streamProviderName));
        _streamProvider = this.GetStreamProvider(streamProviderName);
    }

    /// <inheritdoc />
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
        _subscription = await GetStream().SubscribeAsync(HandleNextAsync, HandleExceptionAsync, HandCompleteAsync);
    }

    /// <inheritdoc />
    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        if (_subscription != null)
        {
            await _subscription.UnsubscribeAsync();
        }
        var subscriptions = await GetStream().GetAllSubscriptionHandles();
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
    protected abstract Task HandleNextAsync(TEvent domainEvent, StreamSequenceToken sequenceToken);

    /// <summary>
    ///     Handles an exception that occurred while processing the stream.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    protected abstract Task HandleExceptionAsync(Exception exception);

    /// <summary>
    ///     Handles the completion of the stream.
    /// </summary>
    protected abstract Task HandCompleteAsync();
}
