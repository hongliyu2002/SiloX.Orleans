using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.DataManagement;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using JetBrains.Annotations;
using SiloX.Orleans.Persistence.EventStore.Contributors;

namespace SiloX.Orleans.Persistence.EventStore;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<PersistenceModule>]
[DependsOn<HealthChecksModule>]
[DependsOn<DataManagementModule>]
[DependsOn<OpenTelemetryModule>]
[DependsOn<ConfigurationModule>]
public class EventStorePersistenceModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureEventStorePersistenceOptionsContributor>();
        context.Services.AddHealthCheckContributor<EventStorePersistenceHealthChecksContributor>();
        context.Services.AddTracerProviderContributor<TracerProviderContributor>();
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        var eventStoreOptions = context.Services.GetOptions<EventStorePersistenceOptions>();
        context.Log("AddOrleansEventStorePersistence", services => services.AddOrleansEventStorePersistence(eventStoreOptions));
    }
}
