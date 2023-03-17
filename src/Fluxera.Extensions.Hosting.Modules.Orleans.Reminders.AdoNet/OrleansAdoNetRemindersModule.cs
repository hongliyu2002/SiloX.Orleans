using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using Fluxera.Extensions.Hosting.Modules.Orleans.Reminders.AdoNet.Contributors;
using JetBrains.Annotations;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminders.AdoNet;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<ConfigurationModule>]
public class OrleansAdoNetRemindersModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureAdoNetRemindersOptionsContributor>();
        context.Services.AddHealthCheckContributor<AdoNetRemindersHealthChecksContributor>();
        context.Services.AddTracerProviderContributor<TracerProviderContributor>();
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        var remindersOptions = context.Services.GetObject<AdoNetRemindersOptions>();
        context.Log("AddOrleansAdoNetReminders", services => services.AddOrleansAdoNetReminders(remindersOptions));
    }
}
