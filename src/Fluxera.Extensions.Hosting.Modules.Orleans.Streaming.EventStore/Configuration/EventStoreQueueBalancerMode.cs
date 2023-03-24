namespace Fluxera.Extensions.Hosting.Modules.Orleans.Streaming.EventStore;

/// <summary>
/// </summary>
public enum EventStoreQueueBalancerMode
{
    /// <summary>
    /// </summary>
    ConsistentRing,

    /// <summary>
    /// </summary>
    LeaseBased,

    /// <summary>
    /// </summary>
    DynamicClusterConfigDeployment,

    /// <summary>
    /// </summary>
    StaticClusterConfigDeployment
}
