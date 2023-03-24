using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Clustering.Redis.Contributors;

internal sealed class RedisClusteringHealthChecksContributor : IHealthChecksContributor
{
    /// <inheritdoc />
    public void ConfigureHealthChecks(IHealthChecksBuilder builder, IServiceConfigurationContext context)
    {
        var clusteringOptions = context.Services.GetObject<RedisClusteringOptions>();
        clusteringOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        if (clusteringOptions.ConnectionStrings.TryGetValue(clusteringOptions.ProviderName, out var connectionString))
        {
            builder.AddRedis(connectionString, "RedisClustering", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
        }
    }
}
