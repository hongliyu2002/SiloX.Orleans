using Fluxera.Extensions.DataManagement;
using JetBrains.Annotations;

namespace SiloX.Orleans.Clustering.Redis;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class RedisClusteringOptions
{
    /// <summary>
    ///     The name of the provider (also used as connection string name).
    /// </summary>
    public string ProviderName { get; set; } = "RedisCluster";

    /// <summary>
    ///     Gets the connection strings.
    /// </summary>
    [Redact]
    public ConnectionStrings ConnectionStrings { get; internal set; } = new();
}
