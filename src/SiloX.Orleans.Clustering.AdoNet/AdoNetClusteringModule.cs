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
[DependsOn<ClusteringModule>]
[DependsOn<HealthChecksModule>]
[DependsOn<DataManagementModule>]
[DependsOn<OpenTelemetryModule>]
[DependsOn<ConfigurationModule>]
public class AdoNetClusteringModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureAdoNetClusteringOptionsContributor>();
        context.Services.AddHealthCheckContributor<AdoNetClusteringHealthChecksContributor>();
        context.Services.AddTracerProviderContributor<TracerProviderContributor>();
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        var options = context.Services.GetOptions<ClusteringOptions>();
        var adoNetOptions = context.Services.GetOptions<AdoNetClusteringOptions>();
        context.Log("AddOrleansAdoNetClustering", services => services.AddOrleansAdoNetClustering(options, adoNetOptions));
    }
}
