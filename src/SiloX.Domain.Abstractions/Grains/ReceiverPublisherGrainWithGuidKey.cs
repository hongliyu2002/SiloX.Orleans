﻿using Fluxera.Guards;
using Orleans.Runtime;
using Orleans.Streams;

namespace SiloX.Domain.Abstractions;

/// <summary>
///     Represents a base class for Orleans event subscribers that can subscribe to a stream of TEvent from Broadcast.
/// </summary>
public abstract class ReceiverPublisherGrainWithGuidKey<TSubEvent, TSubErrorEvent, TPubEvent, TPubErrorEvent> : Grain, IGrainWithGuidKey
    where TSubEvent : DomainEvent
    where TSubErrorEvent : TSubEvent, IDomainErrorEvent
    where TPubEvent : DomainEvent
    where TPubErrorEvent : TPubEvent, IDomainErrorEvent
{
    private readonly IStreamProvider _subStreamProvider;
    private readonly IStreamProvider _pubStreamProvider;
    private IAsyncStream<TSubEvent>? _subBroadcastStream;
    private IAsyncStream<TPubEvent>? _pubStream;
    private IAsyncStream<TPubEvent>? _pubBroadcastStream;

    private StreamSubscriptionHandle<TSubEvent>? _subscription;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ReceiverPublisherGrainWithGuidKey{TSubEvent, TSubErrorEvent, TPubEvent, TPubErrorEvent}" /> class with
    ///     the specified stream provider name and stream namespace.
    /// </summary>
    /// <param name="streamProviderName">The name of the stream provider.</param>
    protected ReceiverPublisherGrainWithGuidKey(string streamProviderName)
    {
        streamProviderName = Guard.Against.NullOrWhiteSpace(streamProviderName, nameof(streamProviderName));
        _subStreamProvider = this.GetStreamProvider(streamProviderName);
        _pubStreamProvider = this.GetStreamProvider(streamProviderName);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ReceiverPublisherGrainWithGuidKey{TSubEvent, TSubErrorEvent, TPubEvent, TPubErrorEvent}" /> class with
    ///     the specified stream provider name and stream namespace.
    /// </summary>
    /// <param name="subStreamProviderName">The name of the stream provider for subscription.</param>
    /// <param name="pubStreamProviderName">The name of the stream provider for publication.</param>
    protected ReceiverPublisherGrainWithGuidKey(string subStreamProviderName, string pubStreamProviderName)
    {
        subStreamProviderName = Guard.Against.NullOrWhiteSpace(subStreamProviderName, nameof(subStreamProviderName));
        pubStreamProviderName = Guard.Against.NullOrWhiteSpace(pubStreamProviderName, nameof(pubStreamProviderName));
        _subStreamProvider = this.GetStreamProvider(subStreamProviderName);
        _pubStreamProvider = this.GetStreamProvider(pubStreamProviderName);
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
    ///     Gets the publisher stream namespace.
    /// </summary>
    /// <returns>The stream namespace.</returns>
    protected abstract string GetPubStreamNamespace();

    /// <summary>
    ///     Gets the publisher stream namespace for the broadcast stream.
    /// </summary>
    /// <returns>The stream namespace.</returns>
    protected abstract string GetPubBroadcastStreamNamespace();

    /// <summary>
    ///     Gets the broadcast stream for the grain.
    /// </summary>
    /// <returns>The stream.</returns>
    private IAsyncStream<TSubEvent> GetSubBroadcastStream()
    {
        return _subBroadcastStream ??= _subStreamProvider.GetStream<TSubEvent>(StreamId.Create(GetSubBroadcastStreamNamespace(), Guid.Empty));
    }

    /// <summary>
    ///     Gets the publisher stream for the grain.
    /// </summary>
    /// <returns>The stream.</returns>
    private IAsyncStream<TPubEvent> GetPubStream()
    {
        return _pubStream ??= _pubStreamProvider.GetStream<TPubEvent>(StreamId.Create(GetPubStreamNamespace(), this.GetPrimaryKey()));
    }

    /// <summary>
    ///     Gets the publisher broadcast stream for the grain.
    /// </summary>
    /// <returns>The stream.</returns>
    private IAsyncStream<TPubEvent> GetPubBroadcastStream()
    {
        return _pubBroadcastStream ??= _pubStreamProvider.GetStream<TPubEvent>(StreamId.Create(GetPubBroadcastStreamNamespace(), Guid.Empty));
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

    /// <summary>
    ///     Publishes a domain event to the stream after domain command has been successfully persisted.
    /// </summary>
    /// <param name="event">The domain event to publish and broadcast.</param>
    /// <param name="broadcastOnly">Whether to broadcast the event.</param>
    protected async Task PublishAsync(TPubEvent @event, bool broadcastOnly = true)
    {
        if (!broadcastOnly)
        {
            await GetPubStream().OnNextAsync(@event);
        }
        await GetPubBroadcastStream().OnNextAsync(@event);
    }

    /// <summary>
    ///     Publishes a domain error event to the stream.
    /// </summary>
    /// <param name="errorEvent">The domain error event to publish and broadcast.</param>
    /// <param name="broadcastOnly">Whether to broadcast the event.</param>
    protected async Task PublishErrorAsync(TPubErrorEvent errorEvent, bool broadcastOnly = true)
    {
        if (!broadcastOnly)
        {
            await GetPubStream().OnNextAsync(errorEvent);
        }
        await GetPubBroadcastStream().OnNextAsync(errorEvent);
    }
}