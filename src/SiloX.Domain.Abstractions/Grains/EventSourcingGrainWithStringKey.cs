using Fluxera.Guards;
using Orleans.EventSourcing;
using Orleans.Runtime;
using Orleans.Streams;

namespace SiloX.Domain.Abstractions;

/// <summary>
///     Provides a base class for grains that use event sourcing for persistence.
/// </summary>
/// <typeparam name="TState">The type of state used by the grain.</typeparam>
/// <typeparam name="TCommand">The type of domain command used by the grain.</typeparam>
/// <typeparam name="TPubEvent">The type of domain event used by the grain.</typeparam>
/// <typeparam name="TPubErrorEvent">The type of domain error event used by the grain.</typeparam>
public abstract class EventSourcingGrainWithStringKey<TState, TCommand, TPubEvent, TPubErrorEvent> : JournaledGrain<TState, TCommand>, IGrainWithStringKey
    where TState : class, new()
    where TCommand : DomainCommand
    where TPubEvent : DomainEvent
    where TPubErrorEvent : TPubEvent, IDomainErrorEvent
{
    private readonly IStreamProvider _pubStreamProvider;
    private IAsyncStream<TPubEvent>? _pubStream;
    private IAsyncStream<TPubEvent>? _pubBroadcastStream;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EventSourcingGrainWithStringKey{TState, TCommand, TPubEvent, TPubErrorEvent}" /> class.
    /// </summary>
    /// <param name="streamProviderName">The name of the stream provider.</param>
    protected EventSourcingGrainWithStringKey(string streamProviderName)
    {
        streamProviderName = Guard.Against.NullOrWhiteSpace(streamProviderName, nameof(streamProviderName));
        _pubStreamProvider = this.GetStreamProvider(streamProviderName);
    }

    /// <summary>
    ///     Gets the stream namespace for the publish stream.
    /// </summary>
    /// <returns>The stream namespace.</returns>
    protected abstract string GetPubStreamNamespace();

    /// <summary>
    ///     Gets the stream namespace for the broadcast stream.
    /// </summary>
    /// <returns>The stream namespace.</returns>
    protected abstract string GetPubBroadcastStreamNamespace();

    /// <summary>
    ///     Gets the stream for the grain.
    /// </summary>
    /// <returns>The stream.</returns>
    private IAsyncStream<TPubEvent> GetPubStream()
    {
        return _pubStream ??= _pubStreamProvider.GetStream<TPubEvent>(StreamId.Create(GetPubStreamNamespace(), this.GetPrimaryKeyString()));
    }

    /// <summary>
    ///     Gets the broadcast stream for the grain.
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
    protected async Task PublishAsync(TPubEvent @event, bool broadcastOnly = false)
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
    protected async Task PublishErrorAsync(TPubErrorEvent errorEvent, bool broadcastOnly = false)
    {
        if (!broadcastOnly)
        {
            await GetPubStream().OnNextAsync(errorEvent);
        }
        await GetPubBroadcastStream().OnNextAsync(errorEvent);
    }
}