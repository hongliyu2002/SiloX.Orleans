using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.DataManagement;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using JetBrains.Annotations;
using SiloX.Orleans.Persistence.AdoNet.Contributors;

namespace SiloX.Orleans.Persistence.AdoNet;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<PersistenceModule>]
[DependsOn<HealthChecksModule>]
[DependsOn<DataManagementModule>]
[DependsOn<OpenTelemetryModule>]
[DependsOn<ConfigurationModule>]
public class AdoNetPersistenceModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureAdoNetPersistenceOptionsContributor>();
        context.Services.AddHealthCheckContributor<AdoNetPersistenceHealthChecksContributor>();
        context.Services.AddTracerProviderContributor<TracerProviderContributor>();
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        var adoNetOptions = context.Services.GetOptions<AdoNetPersistenceOptions>();
        context.Log("AddOrleansAdoNetPersistence", services => services.AddOrleansAdoNetPersistence(adoNetOptions));
    }
}
