using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.EventSourcing.Dev;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    public const string LogProviderDoesNotSupport = "Log consistency provider does not support.";

    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansDevEventSourcing(this IServiceCollection services, DevEventSourcingOptions options)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       foreach (var logConsistency in options.LogConsistencies)
                                       {
                                           switch (logConsistency.LogProvider)
                                           {
                                               case DevLogConsistencyProvider.LogStorageBased:
                                                   siloBuilder.AddLogStorageBasedLogConsistencyProvider(logConsistency.Name);
                                                   break;
                                               case DevLogConsistencyProvider.StateStorageBased:
                                                   siloBuilder.AddStateStorageBasedLogConsistencyProvider(logConsistency.Name);
                                                   break;
                                               case DevLogConsistencyProvider.CustomStorageBased:
                                                   siloBuilder.AddCustomStorageBasedLogConsistencyProvider(logConsistency.Name, logConsistency.PrimaryCluster);
                                                   break;
                                               default:
                                                   throw new ArgumentOutOfRangeException(nameof(logConsistency.LogProvider), LogProviderDoesNotSupport);
                                           }
                                       }
                                   });
    }
}
