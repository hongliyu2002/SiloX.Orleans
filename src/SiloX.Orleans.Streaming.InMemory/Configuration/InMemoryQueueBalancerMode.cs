namespace SiloX.Orleans.Streaming.InMemory;

/// <summary>
/// </summary>
public enum InMemoryQueueBalancerMode
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
