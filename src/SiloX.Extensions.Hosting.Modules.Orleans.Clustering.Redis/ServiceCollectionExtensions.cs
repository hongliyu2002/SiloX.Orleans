using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace SiloX.Extensions.Hosting.Modules.Orleans.Clustering.Redis;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansRedisClustering(this IServiceCollection services, RedisClusteringOptions options)
    {
        if (!options.ConnectionStrings.TryGetValue(options.ProviderName, out var connectionString))
        {
            return services;
        }
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
}
