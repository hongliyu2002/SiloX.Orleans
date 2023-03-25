using System.Text.Json.Serialization;
using Fluxera.Extensions.DataManagement;
using JetBrains.Annotations;
using Orleans.Configuration;
using Orleans.Providers;
using Orleans.Streams;

namespace SiloX.Orleans.Streaming.EventStore;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class EventStoreStreamingOptions
{
    /// <summary>
    ///     The streams options.
    /// </summary>
    public EventStoreStreamingStreamsOptions[] Streams { get; set; } = Array.Empty<EventStoreStreamingStreamsOptions>();

    /// <summary>
    ///     Gets the connection strings.
    /// </summary>
    [Redact]
    public ConnectionStrings ConnectionStrings { get; internal set; } = new();
}

/// <summary>
/// </summary>
[PublicAPI]
public sealed class EventStoreStreamingStreamsOptions
{
    /// <summary>
    ///     The name of the provider (also used as connection string name).
    /// </summary>
    public string ProviderName { get; set; } = ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME;

    /// <summary>
    ///     The user name of credentials that have permissions to append events.
    /// </summary>
    [Redact]
    public string? Username { get; set; }

    /// <summary>
    ///     The password of credentials that have permissions to append events.
    /// </summary>
    [Redact]
    public string? Password { get; set; }

    /// <summary>
    ///     EventStore name for this connection, used in cache monitor.
    /// </summary>
    public string Name { get; set; } = ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME;

    /// <summary>
    ///     The queue names (aka stream names) of EventStore.
    /// </summary>
    public List<string> Queues { get; set; } = new();

    #region Receiver Options

    /// <summary>
    ///     Whether the <see cref="T:EventStore.Client.PersistentSubscription"></see> should resolve linkTo events to their linked events.
    /// </summary>
    public bool SubscriptionResolveLinkTos { get; set; }

    /// <summary>
    ///     Whether to track latency statistics on this subscription.
    /// </summary>
    public bool SubscriptionExtraStatistics { get; set; }

    /// <summary>
    ///     The amount of time after which to consider a message as timed out and retried.
    /// </summary>
    public TimeSpan? SubscriptionMessageTimeout { get; set; }

    /// <summary>
    ///     The maximum number of retries (due to timeout) before a message is considered to be parked.
    /// </summary>
    public int SubscriptionMaxRetryCount { get; set; } = 10;

    /// <summary>
    ///     The size of the buffer (in-memory) listening to live messages as they happen before paging occurs.
    /// </summary>
    public int SubscriptionLiveBufferSize { get; set; } = 500;

    /// <summary>
    ///     The number of events read at a time when paging through history.
    /// </summary>
    public int SubscriptionReadBatchSize { get; set; } = 20;

    /// <summary>
    ///     The number of events to cache when paging through history.
    /// </summary>
    public int SubscriptionHistoryBufferSize { get; set; } = 500;

    /// <summary>
    ///     The amount of time to try to checkpoint after.
    /// </summary>
    public TimeSpan SubscriptionCheckPointAfter { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    ///     The minimum number of messages to process before a checkpoint may be written.
    /// </summary>
    public int SubscriptionCheckPointLowerBound { get; set; } = 1;

    /// <summary>
    ///     The maximum number of messages not checkpointed before forcing a checkpoint.
    /// </summary>
    public int SubscriptionCheckPointUpperBound { get; set; } = 1000;

    /// <summary>
    ///     The maximum number of subscribers allowed.
    /// </summary>
    public int SubscriptionMaxSubscriberCount { get; set; }

    /// <summary>
    ///     The strategy to use for distributing events to client consumers. See <see cref="T:EventStore.Client.SystemConsumerStrategies" /> for system supported strategies.
    /// </summary>
    public string SubscriptionConsumerStrategyName { get; set; } = "RoundRobin";

    /// <summary>
    ///     Optional parameter that configures the receiver prefetch count.
    /// </summary>
    public int PrefetchCount { get; set; } = 10;

    /// <summary>
    ///     In cases where no checkpoint is found,
    ///     this indicates if service should read from the most recent data, or from the beginning of a stream.
    /// </summary>
    public bool StartFromNow { get; set; } = true;

    #endregion

    #region Stream Cache Pressure Options

    /// <summary>
    ///     Slow consuming pressure monitor config.
    /// </summary>
    public double? SlowConsumingMonitorFlowControlThreshold { get; set; }

    /// <summary>
    ///     Slow consuming monitor pressure windowsize.
    /// </summary>
    public TimeSpan? SlowConsumingMonitorPressureWindowSize { get; set; }

    /// <summary>
    ///     AveragingCachePressureMonitorFlowControlThreshold, AveragingCachePressureMonitor is turn on by default.
    ///     User can turn it off by setting this value to null
    /// </summary>
    public double? AveragingCachePressureMonitorFlowControlThreshold { get; set; } = EventStoreStreamCachePressureOptions.DefaultAveragingCachePressureMonitoringThreshold;

    #endregion

    #region Stream Pub/Sub Options

    /// <summary>
    ///     The pub sub type.
    /// </summary>
    /// <value>The type of the pub sub.</value>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StreamPubSubType PubSubType { get; set; } = StreamPubSubOptions.DEFAULT_STREAM_PUBSUB_TYPE;

    #endregion

    #region Stream Lifecycle Options

    /// <summary>
    ///     The lifecycle stage at which to initialize the stream runtime.
    /// </summary>
    /// <value>The initialization stage.</value>
    public int InitStage { get; set; } = ServiceLifecycleStage.ApplicationServices;

    /// <summary>
    ///     The lifecycle stage at which to start the stream runtime.
    /// </summary>
    /// <value>The startup stage.</value>
    public int StartStage { get; set; } = ServiceLifecycleStage.Active;

    /// <summary>
    ///     If set to <see cref="StreamLifecycleOptions.RunState.AgentsStarted" />, stream pulling agents will be started during initialization.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StreamLifecycleOptions.RunState StartupState { get; set; } = StreamLifecycleOptions.RunState.AgentsStarted;

    #endregion

    #region Stream Cache Eviction Options

    /// <summary>
    ///     The minimum time a message will stay in cache before it is available for time based purge.
    /// </summary>
    public TimeSpan DataMinTimeInCache { get; set; } = StreamCacheEvictionOptions.DefaultDataMinTimeInCache;

    /// <summary>
    ///     The difference in time between the newest and oldest messages in the cache.  Any messages older than this will be purged from the cache.
    /// </summary>
    public TimeSpan DataMaxAgeInCache { get; set; } = StreamCacheEvictionOptions.DefaultDataMaxAgeInCache;

    /// <summary>
    ///     The minimum time that message metadata (<see cref="StreamSequenceToken" />) will stay in cache before it is available for time based purge.
    ///     Used to avoid cache miss if the full message was purged.
    ///     Set to <see langword="null" /> to disable this tracking.
    /// </summary>
    public TimeSpan? MetadataMinTimeInCache { get; set; } = StreamCacheEvictionOptions.DefaultMetadataMinTimeInCache;

    #endregion

    #region Stream Statistic Options

    /// <summary>
    ///     The statistic monitor write interval.
    ///     Statistics generation is triggered by activity. Interval will be ignored when streams are inactive.
    /// </summary>
    public TimeSpan StatisticMonitorWriteInterval { get; set; } = StreamStatisticOptions.DefaultStatisticMonitorWriteInterval;

    #endregion

    #region Stream Pulling Agent Options

    /// <summary>
    ///     The size of each batch container batch.
    /// </summary>
    /// <value>The size of each batch container batch.</value>
    public int BatchContainerBatchSize { get; set; } = StreamPullingAgentOptions.DEFAULT_BATCH_CONTAINER_BATCH_SIZE;

    /// <summary>
    ///     The period between polling for queue messages.
    /// </summary>
    public TimeSpan GetQueueMsgsTimerPeriod { get; set; } = StreamPullingAgentOptions.DEFAULT_GET_QUEUE_MESSAGES_TIMER_PERIOD;

    /// <summary>
    ///     The queue initialization timeout.
    /// </summary>
    /// <value>The queue initialization timeout.</value>
    public TimeSpan InitQueueTimeout { get; set; } = StreamPullingAgentOptions.DEFAULT_INIT_QUEUE_TIMEOUT;

    /// <summary>
    ///     The maximum event delivery time.
    /// </summary>
    /// <value>The maximum event delivery time.</value>
    public TimeSpan MaxEventDeliveryTime { get; set; } = StreamPullingAgentOptions.DEFAULT_MAX_EVENT_DELIVERY_TIME;

    /// <summary>
    ///     The stream inactivity period.
    /// </summary>
    /// <value>The stream inactivity period.</value>
    public TimeSpan StreamInactivityPeriod { get; set; } = StreamPullingAgentOptions.DEFAULT_STREAM_INACTIVITY_PERIOD;

    #endregion

    /// <summary>
    ///     What kind of the queue balancer mode to use.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EventStoreQueueBalancerMode QueueBalancerMode { get; set; } = EventStoreQueueBalancerMode.ConsistentRing;

    #region Lease Based Queue Balancer Options

    /// <summary>
    ///     The length of the lease.
    /// </summary>
    /// <value>The length of the lease.</value>
    public TimeSpan LeaseLength { get; set; } = LeaseBasedQueueBalancerOptions.DefaultLeaseLength;

    /// <summary>
    ///     The lease renew period.
    /// </summary>
    /// <value>The lease renew period.</value>
    /// <remarks>
    ///     <see cref="LeaseBasedQueueBalancerOptions.DefaultLeaseRenewPeriod" /> set to (<see cref="LeaseBasedQueueBalancerOptions.DefaultLeaseLength" />/2 - 1) to allow time for at least 2 renew calls before we lose the lease.
    /// </remarks>
    public TimeSpan LeaseRenewPeriod { get; set; } = LeaseBasedQueueBalancerOptions.DefaultLeaseRenewPeriod;

    /// <summary>
    ///     How often balancer attempts to aquire leases.
    /// </summary>
    public TimeSpan LeaseAquisitionPeriod { get; set; } = LeaseBasedQueueBalancerOptions.DefaultMinLeaseAquisitionPeriod;

    /// <summary>
    ///     The lease category, allows for more fine grain partitioning of leases.
    /// </summary>
    public string LeaseCategory { get; set; } = LeaseBasedQueueBalancerOptions.DefaultLeaseCategory;

    #endregion

    #region Deployment Based Queue Balancer Options

    /// <summary>
    ///     The silo maturity period, which is the period of time to allow a silo to remain active for before rebalancing queues.
    /// </summary>
    /// <value>The silo maturity period.</value>
    public TimeSpan SiloMaturityPeriod { get; set; } = DeploymentBasedQueueBalancerOptions.DEFAULT_SILO_MATURITY_PERIOD;

    #endregion

    #region Stream Checkpointer Options

    /// <summary>
    ///     The name of the provider (also used as connection string name).
    /// </summary>
    public string CheckpointerProviderName { get; set; } = ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME;

    /// <summary>
    ///     The user name of credentials that have permissions to append events.
    /// </summary>
    [Redact]
    public string? CheckpointerUsername { get; set; }

    /// <summary>
    ///     The password of credentials that have permissions to append events.
    /// </summary>
    [Redact]
    public string? CheckpointerPassword { get; set; }

    /// <summary>
    ///     Interval to write checkpoints.  Prevents spamming storage.
    /// </summary>
    public TimeSpan CheckpointerPersistInterval { get; set; } = EventStoreStreamCheckpointerOptions.DefaultCheckpointPersistInterval;

    #endregion

}
