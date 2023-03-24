using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.InMemory;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansInMemoryPersistence(this IServiceCollection services, InMemoryPersistenceOptions options)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       foreach (var storage in options.StorageOptions)
                                       {
                                           siloBuilder.AddMemoryGrainStorage(storage.Name,
                                                                             persistence =>
                                                                             {
                                                                                 persistence.NumStorageGrains = storage.NumStorageGrains;
                                                                                 persistence.InitStage = storage.InitStage;
                                                                             });
                                       }
                                   });
    }
}
