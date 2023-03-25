using JetBrains.Annotations;
using Orleans.Configuration;

namespace SiloX.Extensions.Hosting.Modules.Orleans.Clustering;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class ClusteringOptions
{
    /// <summary>
    ///     Is this configuration intended for client-side use?
    /// </summary>
    public bool UsedByClient { get; set; }

    /// <summary>
    ///     A unique identifier for this service, which should survive deployment and redeployment, where as <see cref="ClusterId" /> might not.
    /// </summary>
    public string ServiceId { get; set; } = ClusterOptions.DefaultServiceId;

    /// <summary>
    ///     The cluster identity. This used to be called DeploymentId before Orleans 2.0 name.
    /// </summary>
    public string ClusterId { get; set; } = ClusterOptions.DefaultClusterId;
}
