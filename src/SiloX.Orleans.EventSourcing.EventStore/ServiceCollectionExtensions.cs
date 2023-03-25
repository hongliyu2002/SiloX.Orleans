using EventStore.Client;
using Fluxera.Utilities.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SiloX.Orleans.EventSourcing.EventStore;

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
                                           if (options.ConnectionStrings.TryGetValue(logConsistency.ProviderName, out var connectionString))
                                           {
                                               siloBuilder.AddEventStoreBasedLogConsistencyProvider(logConsistency.ProviderName,
                                                                                                    eventSourcing =>
                                                                                                    {
                                                                                                        eventSourcing.ClientSettings = EventStoreClientSettings.Create(connectionString);
                                                                                                        if (logConsistency.Username.IsNotNullOrEmpty() && logConsistency.Password.IsNotNullOrEmpty())
                                                                                                        {
                                                                                                            eventSourcing.Credentials = new UserCredentials(logConsistency.Username!, logConsistency.Password!);
                                                                                                        }
                                                                                                        eventSourcing.InitStage = logConsistency.InitStage;
                                                                                                    });
                                           }
                                       }
                                   });
    }
}
