using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SiloX.Orleans.EventSourcing.InMemory;

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
    public static IServiceCollection AddOrleansInMemoryEventSourcing(this IServiceCollection services, InMemoryEventSourcingOptions options)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       foreach (var logConsistency in options.LogConsistencies)
                                       {
                                           switch (logConsistency.LogProvider)
                                           {
                                               case InMemoryLogProvider.LogBased:
                                                   siloBuilder.AddLogStorageBasedLogConsistencyProvider(logConsistency.ProviderName);
                                                   break;
                                               case InMemoryLogProvider.StateBased:
                                                   siloBuilder.AddStateStorageBasedLogConsistencyProvider(logConsistency.ProviderName);
                                                   break;
                                               case InMemoryLogProvider.CustomBased:
                                                   siloBuilder.AddCustomStorageBasedLogConsistencyProvider(logConsistency.ProviderName, logConsistency.PrimaryCluster);
                                                   break;
                                               default:
                                                   throw new ArgumentOutOfRangeException(nameof(logConsistency.LogProvider), LogProviderDoesNotSupport);
                                           }
                                       }
                                   });
    }
}
