using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.EventSourcing.EventStore;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansEventStoreEventSourcing(this IServiceCollection services, EventStoreEventSourcingOptions options)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       foreach (var storage in options.Storages)
                                       {
                                           if (options.ConnectionStrings.TryGetValue(storage.ConnectionStringName, out var connectionString))
                                           {
                                               siloBuilder.AddEventStoreBasedLogConsistencyProvider(storage.ConnectionStringName,
                                                                                                    eventSourcing =>
                                                                                                    {
                                                                                                        eventSourcing.ClientSettings = EventStoreClientSettings.Create(connectionString);
                                                                                                        eventSourcing.InitStage = storage.InitStage;
                                                                                                    });
                                           }
                                       }
                                   });
    }
}
