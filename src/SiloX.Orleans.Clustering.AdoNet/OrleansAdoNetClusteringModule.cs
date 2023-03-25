using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.DataManagement;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using JetBrains.Annotations;
using SiloX.Orleans.Clustering.AdoNet.Contributors;

namespace SiloX.Orleans.Clustering.AdoNet;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<OrleansClusteringModule>]
[DependsOn<HealthChecksModule>]
[DependsOn<DataManagementModule>]
[DependsOn<OpenTelemetryModule>]
[DependsOn<ConfigurationModule>]
public class OrleansAdoNetClusteringModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureAdoNetClusteringOptionsContributor>();
        context.Services.AddHealthCheckContributor<AdoNetClusteringHealthChecksContributor>();
        context.Services.AddTracerProviderContributor<TracerProviderContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var clusteringOptions = context.Services.GetObject<AdoNetClusteringOptions>();
        context.Log("AddOrleansAdoNetClustering", services => services.AddOrleansAdoNetClustering(clusteringOptions));
    }
}
