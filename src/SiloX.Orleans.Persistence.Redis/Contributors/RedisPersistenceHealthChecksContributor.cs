﻿using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SiloX.Orleans.Persistence.Redis.Contributors;

internal sealed class RedisPersistenceHealthChecksContributor : IHealthChecksContributor
{
    /// <inheritdoc />
    public void ConfigureHealthChecks(IHealthChecksBuilder builder, IServiceConfigurationContext context)
    {
        var redisOptions = context.Services.GetOptions<RedisPersistenceOptions>();
        redisOptions.ConnectionStrings = context.Services.GetOptions<ConnectionStrings>();
        foreach (var storage in redisOptions.Storages)
        {
            if (redisOptions.ConnectionStrings.TryGetValue(storage.ProviderName, out var connectionString))
            {
                builder.AddRedis(connectionString, storage.ProviderName, HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
            }
        }
    }
}
