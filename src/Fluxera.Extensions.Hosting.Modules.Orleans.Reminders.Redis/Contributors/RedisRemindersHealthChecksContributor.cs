﻿using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminders.Redis.Contributors;

internal sealed class RedisRemindersHealthChecksContributor : IHealthChecksContributor
{
    /// <inheritdoc />
    public void ConfigureHealthChecks(IHealthChecksBuilder builder, IServiceConfigurationContext context)
    {
        var remindersOptions = context.Services.GetObject<RedisRemindersOptions>();
        remindersOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        if (remindersOptions.ConnectionStrings.TryGetValue(remindersOptions.ConnectionStringName, out var connectionString))
        {
            builder.AddRedis(connectionString, "RedisReminders", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
        }
    }
}