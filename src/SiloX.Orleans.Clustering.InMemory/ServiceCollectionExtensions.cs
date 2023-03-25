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
    /// <returns></returns>
    public static IServiceCollection AddOrleansInMemoryClustering(this IServiceCollection services, InMemoryClusteringOptions options)
    {
        if (options.UsedByClient)
        {
            return services.AddOrleansClient(clientBuilder =>
                                             {
                                                 clientBuilder.UseLocalhostClustering(options.GatewayPort, options.ServiceId, options.ClusterId);
                                             });
        }
        return services.AddOrleans(siloBuilder =>
                                   {
                                       siloBuilder.UseLocalhostClustering(options.SiloPort, options.GatewayPort, null, options.ServiceId, options.ClusterId);
                                   });
    }
}
