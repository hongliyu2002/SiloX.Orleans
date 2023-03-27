using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using SiloX.Orleans.Clustering.InMemory.Contributors;

namespace SiloX.Orleans.Clustering.InMemory;

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
        var options = context.Services.GetOptions<ClusteringOptions>();
        var inMemoryOptions = context.Services.GetOptions<InMemoryClusteringOptions>();
        context.Log("AddOrleansInMemoryClustering", services => services.AddOrleansInMemoryClustering(options, inMemoryOptions));
    }
}
