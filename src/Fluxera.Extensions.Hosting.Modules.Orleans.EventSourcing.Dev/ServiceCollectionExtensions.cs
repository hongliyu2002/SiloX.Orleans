using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.EventSourcing.Dev;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    public const string LogProviderNotSupports = "Log consistency provider does not support.";

    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansDevEventSourcing(this IServiceCollection services, DevEventSourcingOptions options)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       foreach (var provider in options.LogConsistencyProviders)
                                       {
                                           switch (provider.Provider)
                                           {
                                               case DevLogConsistencyProvider.LogStorageBased:
                                                   siloBuilder.AddLogStorageBasedLogConsistencyProvider(provider.Name);
                                                   break;
                                               case DevLogConsistencyProvider.StateStorageBased:
                                                   siloBuilder.AddStateStorageBasedLogConsistencyProvider(provider.Name);
                                                   break;
                                               case DevLogConsistencyProvider.CustomStorageBased:
                                                   siloBuilder.AddCustomStorageBasedLogConsistencyProvider(provider.Name, provider.PrimaryCluster);
                                                   break;
                                               default:
                                                   throw new ArgumentOutOfRangeException(nameof(provider.Provider), LogProviderNotSupports);
                                           }
                                       }
                                   });
    }
}
