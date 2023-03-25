using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

namespace SiloX.Extensions.Hosting.Modules.Orleans.Clustering;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansClustering(this IServiceCollection services, ClusteringOptions options)
    {
        if (options.UsedByClient)
        {
            return services.AddOrleansClient(clientBuilder =>
                                             {
                                                 clientBuilder.Configure<ClusterOptions>(cluster =>
                                                                                         {
                                                                                             cluster.ServiceId = options.ServiceId;
                                                                                             cluster.ClusterId = options.ClusterId;
                                                                                         });
                                             });
        }
        return services.AddOrleans(siloBuilder =>
                                   {
                                       siloBuilder.Configure<ClusterOptions>(cluster =>
                                                                             {
                                                                                 cluster.ServiceId = options.ServiceId;
                                                                                 cluster.ClusterId = options.ClusterId;
                                                                             });
                                   });
    }
}
