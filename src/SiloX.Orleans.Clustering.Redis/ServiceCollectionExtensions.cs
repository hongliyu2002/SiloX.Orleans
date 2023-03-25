using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace SiloX.Orleans.Clustering.Redis;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <param name="redisOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansRedisClustering(this IServiceCollection services, ClusteringOptions options, RedisClusteringOptions redisOptions)
    {
        if (redisOptions.ConnectionStrings.TryGetValue(redisOptions.ProviderName, out var connectionString))
        {
            if (options.UsedByClient)
            {
                return services.AddOrleansClient(clientBuilder =>
                                                 {
                                                     clientBuilder.UseRedisClustering(clustering =>
                                                                                      {
                                                                                          clustering.ConfigurationOptions = ConfigurationOptions.Parse(connectionString);
                                                                                      });
                                                 });
            }
            return services.AddOrleans(siloBuilder =>
                                       {
                                           siloBuilder.UseRedisClustering(clustering =>
                                                                          {
                                                                              clustering.ConfigurationOptions = ConfigurationOptions.Parse(connectionString);
                                                                          });
                                       });
        }
        return services;
    }
}
