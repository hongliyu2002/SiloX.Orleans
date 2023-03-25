using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SiloX.Orleans.Persistence.InMemory;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="inMemoryOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansInMemoryPersistence(this IServiceCollection services, InMemoryPersistenceOptions inMemoryOptions)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       foreach (var storage in inMemoryOptions.Storages)
                                       {
                                           siloBuilder.AddMemoryGrainStorage(storage.ProviderName,
                                                                             persistence =>
                                                                             {
                                                                                 persistence.NumStorageGrains = storage.NumStorageGrains;
                                                                                 persistence.InitStage = storage.InitStage;
                                                                             });
                                       }
                                   });
    }
}
