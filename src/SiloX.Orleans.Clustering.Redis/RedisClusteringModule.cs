﻿using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.DataManagement;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using JetBrains.Annotations;
using SiloX.Orleans.Clustering.Redis.Contributors;

namespace SiloX.Orleans.Clustering.Redis;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<ClusteringModule>]
[DependsOn<HealthChecksModule>]
[DependsOn<DataManagementModule>]
[DependsOn<OpenTelemetryModule>]
[DependsOn<ConfigurationModule>]
public class RedisClusteringModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureRedisClusteringOptionsContributor>();
        context.Services.AddHealthCheckContributor<RedisClusteringHealthChecksContributor>();
        context.Services.AddTracerProviderContributor<TracerProviderContributor>();
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        var options = context.Services.GetOptions<ClusteringOptions>();
        var redisOptions = context.Services.GetOptions<RedisClusteringOptions>();
        context.Log("AddOrleansRedisClustering", services => services.AddOrleansRedisClustering(options, redisOptions));
    }
}
