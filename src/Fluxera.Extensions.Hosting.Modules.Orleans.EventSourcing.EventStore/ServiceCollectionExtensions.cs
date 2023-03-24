using EventStore.Client;
using Fluxera.Utilities.Extensions;
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
                                       foreach (var logConsistency in options.LogConsistencyOptions)
                                       {
                                           if (options.ConnectionStrings.TryGetValue(logConsistency.ProviderName, out var connectionString))
                                           {
                                               siloBuilder.AddEventStoreBasedLogConsistencyProvider(logConsistency.ProviderName,
                                                                                                    eventSourcing =>
                                                                                                    {
                                                                                                        eventSourcing.ClientSettings = EventStoreClientSettings.Create(connectionString);
                                                                                                        if (logConsistency is { Username: { }, Password: { } } && logConsistency.Username.IsNotNullOrEmpty() && logConsistency.Password.IsNotNullOrEmpty())
                                                                                                        {
                                                                                                            eventSourcing.Credentials = new UserCredentials(logConsistency.Username, logConsistency.Password);
                                                                                                        }
                                                                                                        eventSourcing.InitStage = logConsistency.InitStage;
                                                                                                    });
                                           }
                                       }
                                   });
    }
}
