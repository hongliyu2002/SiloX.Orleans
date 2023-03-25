using JetBrains.Annotations;
using Orleans.Configuration;

namespace SiloX.Orleans.Clustering.InMemory;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class InMemoryClusteringOptions
{
    /// <summary>
    ///     Is this configuration intended for client-side use?
    /// </summary>
    public bool UsedByClient { get; set; }

    /// <summary>
    ///     A unique identifier for this service, which should survive deployment and redeployment, where as <see cref="ClusterId" /> might not.
    /// </summary>
    public string ServiceId { get; internal set; } = ClusterOptions.DefaultServiceId;

    /// <summary>
    ///     The cluster identity. This used to be called DeploymentId before Orleans 2.0 name.
    /// </summary>
    public string ClusterId { get; internal set; } = ClusterOptions.DefaultClusterId;

    /// <summary>
    ///     The port this silo uses for silo-to-silo communication.
    /// </summary>
    public int SiloPort { get; internal set; } = 11111;

    /// <summary>
    ///     The port this silo uses for silo-to-client (gateway) communication. Specify 0 to disable gateway functionality.
    /// </summary>
    public int GatewayPort { get; internal set; } = 30000;
}
