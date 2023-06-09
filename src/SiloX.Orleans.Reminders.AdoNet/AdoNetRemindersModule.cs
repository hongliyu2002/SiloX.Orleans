﻿using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.DataManagement;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using JetBrains.Annotations;
using SiloX.Orleans.Reminders.AdoNet.Contributors;

namespace SiloX.Orleans.Reminders.AdoNet;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<RemindersModule>]
[DependsOn<HealthChecksModule>]
[DependsOn<DataManagementModule>]
[DependsOn<OpenTelemetryModule>]
[DependsOn<ConfigurationModule>]
public class AdoNetRemindersModule : ConfigureServicesModule
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
        var adoNetOptions = context.Services.GetOptions<AdoNetRemindersOptions>();
        context.Log("AddOrleansAdoNetReminders", services => services.AddOrleansAdoNetReminders(adoNetOptions));
    }
}
