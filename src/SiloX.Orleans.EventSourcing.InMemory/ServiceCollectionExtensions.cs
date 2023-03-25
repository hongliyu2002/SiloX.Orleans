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
                                       foreach (var logConsistency in options.LogConsistencyOptions)
                                       {
                                           switch (logConsistency.LogProvider)
                                           {
                                               case InMemoryLogProvider.LogBased:
                                                   siloBuilder.AddLogStorageBasedLogConsistencyProvider(logConsistency.Name);
                                                   break;
                                               case InMemoryLogProvider.StateBased:
                                                   siloBuilder.AddStateStorageBasedLogConsistencyProvider(logConsistency.Name);
                                                   break;
                                               case InMemoryLogProvider.CustomBased:
                                                   siloBuilder.AddCustomStorageBasedLogConsistencyProvider(logConsistency.Name, logConsistency.PrimaryCluster);
                                                   break;
                                               default:
                                                   throw new ArgumentOutOfRangeException(nameof(logConsistency.LogProvider), LogProviderDoesNotSupport);
                                           }
                                       }
                                   });
    }
}
