using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SiloX.Orleans.Clustering.InMemory;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <param name="inMemoryOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansInMemoryClustering(this IServiceCollection services, ClusteringOptions options, InMemoryClusteringOptions inMemoryOptions)
    {
        if (options.UsedByClient)
        {
            return services.AddOrleansClient(clientBuilder =>
                                             {
                                                 clientBuilder.UseLocalhostClustering(inMemoryOptions.LocalGatewayPort, inMemoryOptions.LocalServiceId, inMemoryOptions.LocalClusterId);
                                             });
        }
        return services.AddOrleans(siloBuilder =>
                                   {
                                       siloBuilder.UseLocalhostClustering(inMemoryOptions.LocalSiloPort, inMemoryOptions.LocalGatewayPort, null, inMemoryOptions.LocalServiceId, inMemoryOptions.LocalClusterId);
                                   });
    }
}
