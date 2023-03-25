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
public abstract class EventSourcingGrain<TState> : JournaledGrain<TState, DomainEvent>, IGrainWithGuidKey
    where TState : class, new()
{
    private readonly string _streamProviderName;
    private readonly string _streamNamespace;

    private IStreamProvider _streamProvider = null!;
    private IAsyncStream<DomainEvent> _stream = null!;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EventSourcingGrain{TState}" /> class.
    /// </summary>
    /// <param name="streamProviderName">The name of the stream provider.</param>
    /// <param name="streamNamespace">The namespace of the stream.</param>
    protected EventSourcingGrain(string streamProviderName, string streamNamespace)
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
    }

    /// <summary>
    ///     Publishes a domain event to the stream after the event has been successfully persisted.
    /// </summary>
    /// <param name="domainEvent">The domain event to publish.</param>
    /// <returns>A <see cref="Result{T}" /> that represents the result of the operation.</returns>
    protected Task<Result<bool>> PublishOnPersistedAsync(DomainEvent domainEvent)
    {
        return Result.Ok().MapTryAsync(() => RaiseConditionalEvent(domainEvent)).MapTryIfAsync(raised => raised, _ => PersistAsync(domainEvent)).TapTryAsync(() => _stream.OnNextAsync(domainEvent with { Version = Version }));
    }

    /// <summary>
    ///     Publishes a domain error event to the stream.
    /// </summary>
    /// <param name="errorEvent">The domain error event to publish.</param>
    /// <returns>A <see cref="Result" /> that represents the result of the operation.</returns>
    protected Task<Result> PublishOnErrorAsync(DomainErrorEvent errorEvent)
    {
        return Result.Ok().TapTryAsync(() => _stream.OnNextAsync(errorEvent));
    }

    /// <summary>
    ///     Custom persistence operation such as saving snapshot to database by using EntityFramework.
    /// </summary>
    /// <param name="domainEvent">The domain event to persist.</param>
    /// <returns>A <see cref="Task{TResult}" /> representing the result of the asynchronous operation.</returns>
    protected abstract Task<bool> PersistAsync(DomainEvent domainEvent);
}
