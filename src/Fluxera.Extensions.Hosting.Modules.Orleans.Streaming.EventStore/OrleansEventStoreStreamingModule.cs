﻿using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.DataManagement;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using Fluxera.Extensions.Hosting.Modules.Orleans.Streaming.EventStore.Contributors;
using JetBrains.Annotations;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Streaming.EventStore;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<OrleansStreamingModule>]
[DependsOn<HealthChecksModule>]
[DependsOn<DataManagementModule>]
[DependsOn<OpenTelemetryModule>]
[DependsOn<ConfigurationModule>]
public class OrleansEventStoreStreamingModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureEventStoreStreamingOptionsContributor>();
        context.Services.AddHealthCheckContributor<EventStoreStreamingHealthChecksContributor>();
        context.Services.AddTracerProviderContributor<TracerProviderContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var streamingOptions = context.Services.GetObject<EventStoreStreamingOptions>();
        context.Log("AddOrleansEventStoreStreaming", services => services.AddOrleansEventStoreStreaming(streamingOptions));
    }
}
