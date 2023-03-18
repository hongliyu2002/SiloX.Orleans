using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.Orleans.Clustering.Dev.Contributors;
using JetBrains.Annotations;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Clustering.Dev;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<OrleansClusteringModule>]
[DependsOn<ConfigurationModule>]
public class OrleansDevClusteringModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureDevClusteringOptionsContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var clusteringOptions = context.Services.GetObject<DevClusteringOptions>();
        context.Log("AddOrleansDevClustering", services => services.AddOrleansDevClustering(clusteringOptions));
    }
}
