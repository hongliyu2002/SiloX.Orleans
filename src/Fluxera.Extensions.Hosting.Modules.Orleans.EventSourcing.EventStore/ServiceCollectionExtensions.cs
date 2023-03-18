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
                                       foreach (var logConsistency in options.LogConsistencies)
                                       {
                                           if (options.ConnectionStrings.TryGetValue(logConsistency.ConnectionStringName, out var connectionString))
                                           {
                                               siloBuilder.AddEventStoreBasedLogConsistencyProvider(logConsistency.ConnectionStringName,
                                                                                                    eventSourcing =>
                                                                                                    {
                                                                                                        eventSourcing.ClientSettings = EventStoreClientSettings.Create(connectionString);
                                                                                                        eventSourcing.InitStage = logConsistency.InitStage;
                                                                                                    });
                                           }
                                       }
                                   });
    }
}
