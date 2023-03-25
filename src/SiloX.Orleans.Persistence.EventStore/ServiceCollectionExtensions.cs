using EventStore.Client;
using Fluxera.Utilities.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SiloX.Orleans.Persistence.EventStore;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="eventStoreOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansEventStorePersistence(this IServiceCollection services, EventStorePersistenceOptions eventStoreOptions)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       foreach (var storage in eventStoreOptions.Storages)
                                       {
                                           if (eventStoreOptions.ConnectionStrings.TryGetValue(storage.ProviderName, out var connectionString))
                                           {
                                               siloBuilder.AddEventStoreGrainStorage(storage.ProviderName,
                                                                                     persistence =>
                                                                                     {
                                                                                         persistence.ClientSettings = EventStoreClientSettings.Create(connectionString);
                                                                                         if (storage is { Username: { }, Password: { } } && storage.Username.IsNotNullOrEmpty() && storage.Password.IsNotNullOrEmpty())
                                                                                         {
                                                                                             persistence.Credentials = new UserCredentials(storage.Username, storage.Password);
                                                                                         }
                                                                                         persistence.DeleteStateOnClear = storage.DeleteStateOnClear;
                                                                                         persistence.InitStage = storage.InitStage;
                                                                                     });
                                           }
                                       }
                                   });
    }
}
