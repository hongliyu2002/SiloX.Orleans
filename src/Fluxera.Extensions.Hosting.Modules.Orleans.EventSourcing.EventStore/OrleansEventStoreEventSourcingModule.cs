using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.DataManagement;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using Fluxera.Extensions.Hosting.Modules.Orleans.EventSourcing.EventStore.Contributors;
using JetBrains.Annotations;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.EventSourcing.EventStore;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<OrleansEventSourcingModule>]
[DependsOn<HealthChecksModule>]
[DependsOn<DataManagementModule>]
[DependsOn<OpenTelemetryModule>]
[DependsOn<ConfigurationModule>]
public class OrleansEventStoreEventSourcingModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureEventStoreEventSourcingOptionsContributor>();
        context.Services.AddHealthCheckContributor<EventStoreEventSourcingHealthChecksContributor>();
        context.Services.AddTracerProviderContributor<TracerProviderContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var eventSourcingOptions = context.Services.GetObject<EventStoreEventSourcingOptions>();
        context.Log("AddOrleansEventStoreEventSourcing", services => services.AddOrleansEventStoreEventSourcing(eventSourcingOptions));
    }
}
