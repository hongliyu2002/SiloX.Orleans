using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using SiloX.Extensions.Hosting.Modules.Orleans.Clustering.InMemory.Contributors;
using JetBrains.Annotations;

namespace SiloX.Extensions.Hosting.Modules.Orleans.Clustering.InMemory;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<OrleansClusteringModule>]
[DependsOn<ConfigurationModule>]
public class OrleansInMemoryClusteringModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureInMemoryClusteringOptionsContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var clusteringOptions = context.Services.GetObject<InMemoryClusteringOptions>();
        context.Log("AddOrleansInMemoryClustering", services => services.AddOrleansInMemoryClustering(clusteringOptions));
    }
}
