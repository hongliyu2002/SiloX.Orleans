using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Orleans.Configuration;
using Orleans.Providers;
using Orleans.Streams;

namespace SiloX.Orleans.Streaming.InMemory;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class InMemoryStreamingOptions
{
    /// <summary>
    ///     The streams options.
    /// </summary>
    public InMemoryStreamingStreamsOptions[] Streams { get; set; } = Array.Empty<InMemoryStreamingStreamsOptions>();
}

/// <summary>
/// </summary>
[PublicAPI]
public sealed class InMemoryStreamingStreamsOptions
{
    /// <summary>
    ///     The name of the provider (also used as connection string name).
    /// </summary>
    public string ProviderName { get; set; } = ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME;

    #region Stream Pub/Sub Options

    /// <summary>
    ///     The pub sub type.
    /// </summary>
    /// <value>The type of the pub sub.</value>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StreamPubSubType PubSubType { get; set; } = StreamPubSubOptions.DEFAULT_STREAM_PUBSUB_TYPE;

    #endregion

    #region Hash Ring Stream Queue Mapper Options

    /// <summary>
    ///     The number of queues to use.
    /// </summary>
    public int NumQueues { get; set; } = HashRingStreamQueueMapperOptions.DEFAULT_NUM_QUEUES;

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
    public InMemoryQueueBalancerMode QueueBalancerMode { get; set; } = InMemoryQueueBalancerMode.ConsistentRing;

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

}
