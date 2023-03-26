using Fluxera.Guards;
using Orleans.EventSourcing;
using Orleans.FluentResults;
using Orleans.Runtime;
using Orleans.Streams;

namespace SiloX.Domain.Abstractions;

/// <summary>
///     Provides a base class for grains that use event sourcing for persistence.
/// </summary>
/// <typeparam name="TState">The type of state used by the grain.</typeparam>
/// <typeparam name="TEvent">The type of domain event used by the grain.</typeparam>
/// <typeparam name="TErrorEvent">The type of domain error event used by the grain.</typeparam>
public abstract class EventSourcingGrain<TState, TEvent, TErrorEvent> : JournaledGrain<TState, TEvent>, IGrainWithGuidKey
    where TState : class, new()
    where TEvent : DomainEvent
    where TErrorEvent : TEvent, IDomainErrorEvent
{
    private readonly IStreamProvider _streamProvider;
    private IAsyncStream<TEvent>? _stream;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EventSourcingGrain{TState, TEvent, TErrorEvent}" /> class.
    /// </summary>
    /// <param name="streamProviderName">The name of the stream provider.</param>
    protected EventSourcingGrain(string streamProviderName)
    {
        streamProviderName = Guard.Against.NullOrWhiteSpace(streamProviderName, nameof(streamProviderName));
        _streamProvider = this.GetStreamProvider(streamProviderName);
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
    ///     Publishes a domain event to the stream after the event has been successfully persisted.
    /// </summary>
    /// <param name="domainEvent">The domain event to publish.</param>
    /// <returns>A <see cref="Result{T}" /> that represents the result of the operation.</returns>
    protected Task<Result<bool>> PublishOnPersistedAsync(TEvent domainEvent)
    {
        return Result.Ok()
                     .MapTryAsync(() => RaiseConditionalEvent(domainEvent))
                     .MapTryIfAsync(raised => raised, _ => PersistAsync(domainEvent))
                     .TapTryIfAsync(persisted => persisted, () => GetStream().OnNextAsync(domainEvent with { Version = Version }));
    }

    /// <summary>
    ///     Publishes a domain error event to the stream.
    /// </summary>
    /// <param name="errorEvent">The domain error event to publish.</param>
    /// <returns>A <see cref="Result" /> that represents the result of the operation.</returns>
    protected Task<Result> PublishOnErrorAsync(TErrorEvent errorEvent)
    {
        return Result.Ok().TapTryAsync(() => GetStream().OnNextAsync(errorEvent));
    }

    /// <summary>
    ///     Custom persistence operation such as saving snapshot to database by using EntityFramework.
    /// </summary>
    /// <param name="domainEvent">The domain event to persist.</param>
    /// <returns>A <see cref="Task{TResult}" /> representing the result of the asynchronous operation.</returns>
    protected abstract Task<bool> PersistAsync(TEvent domainEvent);
}
