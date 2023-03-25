using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.DataManagement;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using SiloX.Extensions.Hosting.Modules.Orleans.Persistence.AdoNet.Contributors;
using JetBrains.Annotations;

namespace SiloX.Extensions.Hosting.Modules.Orleans.Persistence.AdoNet;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<OrleansPersistenceModule>]
[DependsOn<HealthChecksModule>]
[DependsOn<DataManagementModule>]
[DependsOn<OpenTelemetryModule>]
[DependsOn<ConfigurationModule>]
public class OrleansAdoNetPersistenceModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureAdoNetPersistenceOptionsContributor>();
        context.Services.AddHealthCheckContributor<AdoNetPersistenceHealthChecksContributor>();
        context.Services.AddTracerProviderContributor<TracerProviderContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var persistenceOptions = context.Services.GetObject<AdoNetPersistenceOptions>();
        context.Log("AddOrleansAdoNetPersistence", services => services.AddOrleansAdoNetPersistence(persistenceOptions));
    }
}
