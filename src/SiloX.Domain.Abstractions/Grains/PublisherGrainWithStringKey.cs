using Fluxera.Guards;
using Orleans.Runtime;
using Orleans.Streams;

namespace SiloX.Domain.Abstractions;

/// <summary>
///     Represents a base class for Orleans event publishers that can publish TPubEvent.
/// </summary>
public abstract class PublisherGrainWithStringKey<TPubEvent, TPubErrorEvent> : Grain, IGrainWithStringKey
    where TPubEvent : DomainEvent
    where TPubErrorEvent : TPubEvent, IDomainErrorEvent
{
    private readonly IStreamProvider _pubStreamProvider;
    private IAsyncStream<TPubEvent>? _pubStream;
    private IAsyncStream<TPubEvent>? _pubBroadcastStream;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PublisherGrainWithStringKey{TPubEvent, TPubErrorEvent}" /> class with
    ///     the specified stream provider name and stream namespace.
    /// </summary>
    /// <param name="streamProviderName">The name of the stream provider for both subscription and publication.</param>
    protected PublisherGrainWithStringKey(string streamProviderName)
    {
        streamProviderName = Guard.Against.NullOrWhiteSpace(streamProviderName, nameof(streamProviderName));
        _pubStreamProvider = this.GetStreamProvider(streamProviderName);
    }

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
    ///     Gets the publisher stream for the grain.
    /// </summary>
    /// <returns>The stream.</returns>
    private IAsyncStream<TPubEvent> GetPubStream()
    {
        return _pubStream ??= _pubStreamProvider.GetStream<TPubEvent>(StreamId.Create(GetPubStreamNamespace(), this.GetPrimaryKeyString()));
    }

    /// <summary>
    ///     Gets the publisher broadcast stream for the grain.
    /// </summary>
    /// <returns>The stream.</returns>
    private IAsyncStream<TPubEvent> GetPubBroadcastStream()
    {
        return _pubBroadcastStream ??= _pubStreamProvider.GetStream<TPubEvent>(StreamId.Create(GetPubBroadcastStreamNamespace(), "Default"));
    }

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