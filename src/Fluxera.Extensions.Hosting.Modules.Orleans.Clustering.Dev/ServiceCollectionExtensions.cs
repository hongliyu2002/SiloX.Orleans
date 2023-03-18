using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Clustering.Dev;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansDevClustering(this IServiceCollection services, DevClusteringOptions options)
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
