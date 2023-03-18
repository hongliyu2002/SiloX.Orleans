using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.Dev;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansDevPersistence(this IServiceCollection services, DevPersistenceOptions options)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       foreach (var storage in options.StorageOptions)
                                       {
                                           siloBuilder.AddMemoryGrainStorage(storage.Name,
                                                                             storageOptions =>
                                                                             {
                                                                                 storageOptions.NumStorageGrains = storage.NumStorageGrains;
                                                                                 storageOptions.InitStage = storage.InitStage;
                                                                             });
                                       }
                                   });
    }
}
