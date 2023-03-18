using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.EventStore;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansEventStorePersistence(this IServiceCollection services, EventStorePersistenceOptions options)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       foreach (var storage in options.Storages)
                                       {
                                           if (options.ConnectionStrings.TryGetValue(storage.ConnectionStringName, out var connectionString))
                                           {
                                               siloBuilder.AddEventStoreGrainStorage(storage.ConnectionStringName,
                                                                                     persistence =>
                                                                                     {
                                                                                         persistence.ClientSettings = EventStoreClientSettings.Create(connectionString);
                                                                                         persistence.DeleteStateOnClear = storage.DeleteStateOnClear;
                                                                                         persistence.InitStage = storage.InitStage;
                                                                                     });
                                           }
                                       }
                                   });
    }
}
