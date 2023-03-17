using Fluxera.Extensions.DataManagement;
using JetBrains.Annotations;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Clustering.Redis;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class RedisClusteringOptions
{
    /// <summary>
    ///     Is this configuration intended for client-side use?
    /// </summary>
    public bool UsedByClient { get; set; }

    /// <summary>
    ///     The name of the connection string.
    /// </summary>
    public string ConnectionStringName { get; set; } = "RedisCluster";

    /// <summary>
    ///     Gets the connection strings.
    /// </summary>
    [Redact]
    public ConnectionStrings ConnectionStrings { get; internal set; } = new();
}
