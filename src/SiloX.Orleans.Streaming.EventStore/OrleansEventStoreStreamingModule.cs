using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.DataManagement;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using JetBrains.Annotations;
using SiloX.Orleans.Streaming.EventStore.Contributors;

namespace SiloX.Orleans.Streaming.EventStore;

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
        var options = context.Services.GetOptions<StreamingOptions>();
        var eventStoreOptions = context.Services.GetOptions<EventStoreStreamingOptions>();
        context.Log("AddOrleansEventStoreStreaming", services => services.AddOrleansEventStoreStreaming(options, eventStoreOptions));
    }
}
