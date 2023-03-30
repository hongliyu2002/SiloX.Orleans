using Fluxera.Guards;
using Orleans.FluentResults;
using Orleans.Runtime;
using Orleans.Streams;

namespace SiloX.Domain.Abstractions;

/// <summary>
///     Provides a base class for grains that use grain storage for persistence.
/// </summary>
/// <typeparam name="TState">The type of state used by the grain.</typeparam>
/// <typeparam name="TEvent">The type of domain event used by the grain.</typeparam>
/// <typeparam name="TErrorEvent">The type of domain error event used by the grain.</typeparam>
public abstract class StatefulGrain<TState, TEvent, TErrorEvent> : Grain<TState>, IGrainWithStringKey
    where TState : class, new()
    where TEvent : DomainEvent
    where TErrorEvent : TEvent, IDomainErrorEvent
{
    private readonly IStreamProvider _streamProvider;
    private IAsyncStream<TEvent>? _stream;
    private IAsyncStream<TEvent>? _broadcastStream;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EventSourcingGrain{TState, TCommand, TEvent, TErrorEvent}" /> class.
    /// </summary>
    /// <param name="streamProviderName">The name of the stream provider.</param>
    protected StatefulGrain(string streamProviderName)
    {
        streamProviderName = Guard.Against.NullOrWhiteSpace(streamProviderName, nameof(streamProviderName));
        _streamProvider = this.GetStreamProvider(streamProviderName);
    }

    /// <summary>
    ///     Gets the stream namespace for the publish stream.
    /// </summary>
    /// <returns>The stream namespace.</returns>
    protected abstract string GetStreamNamespace();

    /// <summary>
    ///     Gets the stream namespace for the broadcast stream.
    /// </summary>
    /// <returns>The stream namespace.</returns>
    protected abstract string GetBroadcastStreamNamespace();

    /// <summary>
    ///     Gets the stream for the grain.
    /// </summary>
    /// <returns>The stream.</returns>
    private IAsyncStream<TEvent> GetStream()
    {
        return _stream ??= _streamProvider.GetStream<TEvent>(StreamId.Create(GetStreamNamespace(), this.GetPrimaryKeyString()));
    }

    /// <summary>
    ///     Gets the broadcast stream for the grain.
    /// </summary>
    /// <returns>The stream.</returns>
    private IAsyncStream<TEvent> GetBroadcastStream()
    {
        return _broadcastStream ??= _streamProvider.GetStream<TEvent>(StreamId.Create(GetBroadcastStreamNamespace(), string.Empty));
    }

    /// <summary>
    ///     Publishes a domain event to the stream after domain command has been successfully persisted.
    /// </summary>
    /// <param name="event">The domain event to publish and broadcast.</param>
    /// <returns>A <see cref="Result{T}" /> that represents the result of the operation.</returns>
    protected async Task PublishAsync(TEvent @event)
    {
        await GetStream().OnNextAsync(@event);
        await GetBroadcastStream().OnNextAsync(@event);
    }

    /// <summary>
    ///     Publishes a domain error event to the stream.
    /// </summary>
    /// <param name="errorEvent">The domain error event to publish and broadcast.</param>
    /// <returns>A <see cref="Result" /> that represents the result of the operation.</returns>
    protected async Task PublishErrorAsync(TErrorEvent errorEvent)
    {
        await GetStream().OnNextAsync(errorEvent);
        await GetBroadcastStream().OnNextAsync(errorEvent);
    }

}
