using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.DataManagement;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using JetBrains.Annotations;
using SiloX.Orleans.Reminders.Redis.Contributors;

namespace SiloX.Orleans.Reminders.Redis;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<RemindersModule>]
[DependsOn<HealthChecksModule>]
[DependsOn<DataManagementModule>]
[DependsOn<OpenTelemetryModule>]
[DependsOn<ConfigurationModule>]
public class RedisRemindersModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureRedisRemindersOptionsContributor>();
        context.Services.AddHealthCheckContributor<RedisRemindersHealthChecksContributor>();
        context.Services.AddTracerProviderContributor<TracerProviderContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var redisOptions = context.Services.GetOptions<RedisRemindersOptions>();
        context.Log("AddOrleansRedisReminders", services => services.AddOrleansRedisReminders(redisOptions));
    }
}
