using System.Reactive.Disposables;
using System.Reactive.Linq;
using Fluxera.Guards;
using Orleans.Runtime;
using Orleans.Streams;

namespace SiloX.Domain.Abstractions.Extensions;

/// <summary>
///     Extensions for <see cref="IGrainBase" />.
/// </summary>
public static class GrainExtensions
{
    /// <summary>
    ///     Get a stream of events from a publisher grain.
    /// </summary>
    /// <param name="grain">The <see cref="IGrainBase" />.</param>
    /// <param name="providerName">The name of the stream provider to use.</param>
    /// <param name="subNamespace">The namespace of the single subscriber stream. This is used to distinguish streams from each other. If you want to have multiple streams of the same type, you can use different namespaces.</param>
    /// <param name="id">The id of the grain. This is used to distinguish streams from each other. If you want to have multiple streams of the same type, you can use different ids.</param>
    /// <param name="sequenceToken">The sequence token to start from. If you want to start from the beginning of the stream, you can pass null.</param>
    /// <returns>An <see cref="IObservable{T}" /> that will emit events as they are received.</returns>
    public static IObservable<(TEvent Event, StreamSequenceToken SequenceToken)> GetSubscriberStreamWithGuidKey<TEvent>(this IGrainBase grain, string providerName, string subNamespace, Guid id, StreamSequenceToken? sequenceToken = null)
        where TEvent : DomainEvent
    {
        Guard.Against.Null(grain, nameof(grain));
        Guard.Against.NullOrWhiteSpace(providerName, nameof(providerName));
        Guard.Against.NullOrWhiteSpace(subNamespace, nameof(subNamespace));
        Guard.Against.Empty(id, nameof(id));
        return Observable.Create<(TEvent, StreamSequenceToken)>(async observer =>
                                                                {
                                                                    try
                                                                    {
                                                                        var streamProvider = grain.GetStreamProvider(providerName);
                                                                        var stream = streamProvider.GetStream<TEvent>(StreamId.Create(subNamespace, id));
                                                                        var subscription = await stream.SubscribeAsync((evt, token) => HandleNextAsync(evt, token, observer), ex => OnErrorAsync(ex, observer), () => OnCompletedAsync(observer), sequenceToken)
                                                                                                       .ConfigureAwait(false);
                                                                        return Disposable.Create(() => HandleUnsubscribeAsync(subscription));
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        observer.OnError(ex);
                                                                        return Disposable.Empty;
                                                                    }
                                                                });
    }

    /// <summary>
    ///     Get a stream of events from a broadcaster grain.
    /// </summary>
    /// <param name="grain">The <see cref="IGrainBase" />.</param>
    /// <param name="providerName">The name of the stream provider to use.</param>
    /// <param name="broadcastNamespace">The namespace of the broadcast stream. This is used to distinguish streams from each type. If you want to have multiple streams of the types, you can use different namespaces.</param>
    /// <param name="sequenceToken">The sequence token to start from. If you want to start from the beginning of the stream, you can pass null.</param>
    /// <typeparam name="TEvent">The type of event to receive. This must be a type that is serializable by Orleans.</typeparam>
    /// <returns>An <see cref="IObservable{T}" /> that will emit events as they are received.</returns>
    public static IObservable<(TEvent Event, StreamSequenceToken SequenceToken)> GetReceiverStreamWithGuidKey<TEvent>(this IGrainBase grain, string providerName, string broadcastNamespace, StreamSequenceToken? sequenceToken = null)
        where TEvent : DomainEvent
    {
        Guard.Against.Null(grain, nameof(grain));
        Guard.Against.NullOrWhiteSpace(providerName, nameof(providerName));
        Guard.Against.NullOrWhiteSpace(broadcastNamespace, nameof(broadcastNamespace));
        return Observable.Create<(TEvent, StreamSequenceToken)>(async observer =>
                                                                {
                                                                    try
                                                                    {
                                                                        var streamProvider = grain.GetStreamProvider(providerName);
                                                                        var stream = streamProvider.GetStream<TEvent>(StreamId.Create(broadcastNamespace, Guid.Empty));
                                                                        var subscription = await stream.SubscribeAsync((evt, token) => HandleNextAsync(evt, token, observer), ex => OnErrorAsync(ex, observer), () => OnCompletedAsync(observer), sequenceToken)
                                                                                                       .ConfigureAwait(false);
                                                                        return Disposable.Create(() => HandleUnsubscribeAsync(subscription));
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        observer.OnError(ex);
                                                                        return Disposable.Empty;
                                                                    }
                                                                });
    }

    /// <summary>
    ///     Get a stream of events from a publisher grain.
    /// </summary>
    /// <param name="grain">The <see cref="IGrainBase" />.</param>
    /// <param name="providerName">The name of the stream provider to use.</param>
    /// <param name="subNamespace">The namespace of the single subscriber stream. This is used to distinguish streams from each other. If you want to have multiple streams of the same type, you can use different namespaces.</param>
    /// <param name="id">The id of the grain. This is used to distinguish streams from each other. If you want to have multiple streams of the same type, you can use different ids.</param>
    /// <param name="sequenceToken">The sequence token to start from. If you want to start from the beginning of the stream, you can pass null.</param>
    /// <returns>An <see cref="IObservable{T}" /> that will emit events as they are received.</returns>
    public static IObservable<(TEvent Event, StreamSequenceToken SequenceToken)> GetSubscriberStreamWithStringKey<TEvent>(this IGrainBase grain, string providerName, string subNamespace, string id, StreamSequenceToken? sequenceToken = null)
        where TEvent : DomainEvent
    {
        Guard.Against.Null(grain, nameof(grain));
        Guard.Against.NullOrWhiteSpace(providerName, nameof(providerName));
        Guard.Against.NullOrWhiteSpace(subNamespace, nameof(subNamespace));
        Guard.Against.NullOrEmpty(id, nameof(id));
        return Observable.Create<(TEvent, StreamSequenceToken)>(async observer =>
                                                                {
                                                                    try
                                                                    {
                                                                        var streamProvider = grain.GetStreamProvider(providerName);
                                                                        var stream = streamProvider.GetStream<TEvent>(StreamId.Create(subNamespace, id));
                                                                        var subscription = await stream.SubscribeAsync((evt, token) => HandleNextAsync(evt, token, observer), ex => OnErrorAsync(ex, observer), () => OnCompletedAsync(observer), sequenceToken)
                                                                                                       .ConfigureAwait(false);
                                                                        return Disposable.Create(() => HandleUnsubscribeAsync(subscription));
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        observer.OnError(ex);
                                                                        return Disposable.Empty;
                                                                    }
                                                                });
    }

    /// <summary>
    ///     Get a stream of events from a broadcaster grain.
    /// </summary>
    /// <param name="grain">The <see cref="IGrainBase" />.</param>
    /// <param name="providerName">The name of the stream provider to use.</param>
    /// <param name="broadcastNamespace">The namespace of the broadcast stream. This is used to distinguish streams from each type. If you want to have multiple streams of the types, you can use different namespaces.</param>
    /// <param name="sequenceToken">The sequence token to start from. If you want to start from the beginning of the stream, you can pass null.</param>
    /// <typeparam name="TEvent">The type of event to receive. This must be a type that is serializable by Orleans.</typeparam>
    /// <returns>An <see cref="IObservable{T}" /> that will emit events as they are received.</returns>
    public static IObservable<(TEvent Event, StreamSequenceToken SequenceToken)> GetReceiverStreamWithStringKey<TEvent>(this IGrainBase grain, string providerName, string broadcastNamespace, StreamSequenceToken? sequenceToken = null)
        where TEvent : DomainEvent
    {
        Guard.Against.Null(grain, nameof(grain));
        Guard.Against.NullOrWhiteSpace(providerName, nameof(providerName));
        Guard.Against.NullOrWhiteSpace(broadcastNamespace, nameof(broadcastNamespace));
        return Observable.Create<(TEvent, StreamSequenceToken)>(async observer =>
                                                                {
                                                                    try
                                                                    {
                                                                        var streamProvider = grain.GetStreamProvider(providerName);
                                                                        var stream = streamProvider.GetStream<TEvent>(StreamId.Create(broadcastNamespace, string.Empty));
                                                                        var subscription = await stream.SubscribeAsync((evt, token) => HandleNextAsync(evt, token, observer), ex => OnErrorAsync(ex, observer), () => OnCompletedAsync(observer), sequenceToken)
                                                                                                       .ConfigureAwait(false);
                                                                        return Disposable.Create(() => HandleUnsubscribeAsync(subscription));
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        observer.OnError(ex);
                                                                        return Disposable.Empty;
                                                                    }
                                                                });
    }

    private static Task HandleNextAsync<TEvent>(TEvent domainEvent, StreamSequenceToken sequenceToken, IObserver<(TEvent, StreamSequenceToken)> observer)
        where TEvent : DomainEvent
    {
        try
        {
            observer.OnNext((domainEvent, sequenceToken));
        }
        catch (Exception ex)
        {
            observer.OnError(ex);
        }
        return Task.CompletedTask;
    }

    private static Task OnErrorAsync<TEvent>(Exception exception, IObserver<(TEvent, StreamSequenceToken)> observer)
        where TEvent : DomainEvent
    {
        observer.OnError(exception);
        return Task.CompletedTask;
    }

    private static Task OnCompletedAsync<TEvent>(IObserver<(TEvent, StreamSequenceToken)> observer)
        where TEvent : DomainEvent
    {
        observer.OnCompleted();
        return Task.CompletedTask;
    }

    private static async void HandleUnsubscribeAsync<TEvent>(StreamSubscriptionHandle<TEvent>? subscription)
        where TEvent : DomainEvent
    {
        if (subscription == null)
        {
            return;
        }
        try
        {
            await subscription.UnsubscribeAsync()
                              .ConfigureAwait(false);
        }
        catch
        {
            // ignored
        }
    }
}