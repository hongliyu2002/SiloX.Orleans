﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace SiloX.Orleans.Persistence.Redis;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="redisOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansRedisPersistence(this IServiceCollection services, RedisPersistenceOptions redisOptions)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       foreach (var storage in redisOptions.Storages)
                                       {
                                           if (redisOptions.ConnectionStrings.TryGetValue(storage.ProviderName, out var connectionString))
                                           {
                                               siloBuilder.AddRedisGrainStorage(storage.ProviderName,
                                                                                persistence =>
                                                                                {
                                                                                    persistence.ConfigurationOptions = ConfigurationOptions.Parse(connectionString);
                                                                                    persistence.DeleteStateOnClear = storage.DeleteStateOnClear;
                                                                                    persistence.InitStage = storage.InitStage;
                                                                                });
                                           }
                                       }
                                   });
    }
}
