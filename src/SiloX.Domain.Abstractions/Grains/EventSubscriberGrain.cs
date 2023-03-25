using Fluxera.Guards;
using Orleans.Runtime;
using Orleans.Streams;

namespace SiloX.Domain.Abstractions;

/// <summary>
///     Represents a base class for Orleans event subscribers that can subscribe to a stream of <see cref="DomainEvent" />.
/// </summary>
public abstract class EventSubscriberGrain : Grain, IGrainWithGuidKey
{
    private readonly string _streamProviderName;
    private readonly string _streamNamespace;

    private IStreamProvider _streamProvider = null!;
    private IAsyncStream<DomainEvent> _stream = null!;
    private StreamSubscriptionHandle<DomainEvent>? _subscription;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EventSubscriberGrain" /> class with the specified stream provider name and stream namespace.
    /// </summary>
    /// <param name="streamProviderName">The name of the stream provider.</param>
    /// <param name="streamNamespace">The namespace of the stream.</param>
    protected EventSubscriberGrain(string streamProviderName, string streamNamespace)
    {
        _streamProviderName = Guard.Against.NullOrWhiteSpace(streamProviderName, nameof(streamProviderName));
        _streamNamespace = Guard.Against.NullOrWhiteSpace(streamNamespace, nameof(streamNamespace));
    }

    /// <inheritdoc />
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
        _streamProvider = this.GetStreamProvider(_streamProviderName);
        _stream = _streamProvider.GetStream<DomainEvent>(StreamId.Create(_streamNamespace, this.GetPrimaryKey()));
        _subscription = await _stream.SubscribeAsync(HandleNextAsync, HandleExceptionAsync, HandCompleteAsync);
    }

    /// <inheritdoc />
    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        await _subscription!.UnsubscribeAsync();
        var subscriptions = await _stream.GetAllSubscriptionHandles();
        if (subscriptions is { Count: > 0 })
        {
            await Task.WhenAll(subscriptions.Select(subscription => subscription.UnsubscribeAsync()));
        }
        await base.OnDeactivateAsync(reason, cancellationToken);
    }

    /// <summary>
    ///     Handles the next event in the stream.
    /// </summary>
    /// <param name="domainEvent">The next event in the stream.</param>
    /// <param name="sequenceToken">The sequence token of the event.</param>
    protected abstract Task HandleNextAsync(DomainEvent domainEvent, StreamSequenceToken sequenceToken);

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
