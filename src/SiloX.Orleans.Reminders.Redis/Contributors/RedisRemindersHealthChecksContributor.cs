﻿using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SiloX.Orleans.Reminders.Redis.Contributors;

internal sealed class RedisRemindersHealthChecksContributor : IHealthChecksContributor
{
    /// <inheritdoc />
    public void ConfigureHealthChecks(IHealthChecksBuilder builder, IServiceConfigurationContext context)
    {
        var redisOptions = context.Services.GetObject<RedisRemindersOptions>();
        redisOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        if (redisOptions.ConnectionStrings.TryGetValue(redisOptions.ProviderName, out var connectionString))
        {
            builder.AddRedis(connectionString, redisOptions.ProviderName, HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
        }
    }
}
