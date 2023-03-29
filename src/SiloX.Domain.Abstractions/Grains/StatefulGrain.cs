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
public abstract class StatefulGrain<TState, TEvent, TErrorEvent> : Grain<TState>, IGrainWithGuidKey
    where TState : class, new()
    where TEvent : DomainEvent
    where TErrorEvent : TEvent, IDomainErrorEvent
{
    private readonly IStreamProvider _publishStreamProvider;
    private IAsyncStream<TEvent>? _publishStream;

    /// <summary>
    ///     Initializes a new instance of the <see cref="StatefulGrain{TState, TEvent, TErrorEvent}" /> class.
    /// </summary>
    /// <param name="streamProviderName">The name of the stream provider.</param>
    protected StatefulGrain(string streamProviderName)
    {
        streamProviderName = Guard.Against.NullOrWhiteSpace(streamProviderName, nameof(streamProviderName));
        _publishStreamProvider = this.GetStreamProvider(streamProviderName);
    }

    /// <summary>
    ///     Gets the stream namespace.
    /// </summary>
    /// <returns>The stream namespace.</returns>
    protected abstract string GetPublishStreamNamespace();

    /// <summary>
    ///     Gets the stream for the grain.
    /// </summary>
    /// <returns>The stream.</returns>
    private IAsyncStream<TEvent> GetPublishStream()
    {
        return _publishStream ??= _publishStreamProvider.GetStream<TEvent>(StreamId.Create(GetPublishStreamNamespace(), this.GetPrimaryKey()));
    }

    /// <summary>
    ///     Publishes a domain event to the stream after domain command has been successfully persisted.
    /// </summary>
    /// <param name="event">The domain event to publish.</param>
    /// <returns>A <see cref="Result" /> that represents the result of the operation.</returns>
    protected Task PublishAsync(TEvent @event)
    {
        return GetPublishStream().OnNextAsync(@event);
    }

    /// <summary>
    ///     Publishes a domain error event to the stream.
    /// </summary>
    /// <param name="errorEvent">The domain error event to publish.</param>
    /// <returns>A <see cref="Result" /> that represents the result of the operation.</returns>
    protected Task PublishErrorAsync(TErrorEvent errorEvent)
    {
        return GetPublishStream().OnNextAsync(errorEvent);
    }

    /// <inheritdoc />
    protected override async Task WriteStateAsync()
    {
        await base.WriteStateAsync();
        await PersistAsync();
    }

    /// <summary>
    ///     Customizes the behavior of the grain when a domain event is received.
    /// </summary>
    protected virtual Task PersistAsync()
    {
        return Task.CompletedTask;
    }
}
