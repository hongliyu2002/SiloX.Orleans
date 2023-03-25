using JetBrains.Annotations;
using Orleans.Configuration;

namespace SiloX.Orleans.Clustering.InMemory;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class InMemoryClusteringOptions
{
    /// <summary>
    ///     A unique identifier for this service, which should survive deployment and redeployment, where as <see cref="LocalClusterId" /> might not.
    /// </summary>
    public string LocalServiceId { get; internal set; } = ClusterOptions.DefaultServiceId;

    /// <summary>
    ///     The cluster identity. This used to be called DeploymentId before Orleans 2.0 name.
    /// </summary>
    public string LocalClusterId { get; internal set; } = ClusterOptions.DefaultClusterId;

    /// <summary>
    ///     The port this silo uses for silo-to-silo communication.
    /// </summary>
    public int LocalSiloPort { get; internal set; } = 11111;

    /// <summary>
    ///     The port this silo uses for silo-to-client (gateway) communication. Specify 0 to disable gateway functionality.
    /// </summary>
    public int LocalGatewayPort { get; internal set; } = 30000;
}
