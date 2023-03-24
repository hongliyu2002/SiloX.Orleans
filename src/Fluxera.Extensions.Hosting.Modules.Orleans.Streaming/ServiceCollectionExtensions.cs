using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Streaming;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansStreaming(this IServiceCollection services, StreamingOptions options)
    {
        if (options.UsedByClient)
        {
            return services.AddOrleansClient(clientBuilder =>
                                             {
                                                 clientBuilder.AddStreaming();
                                             });
        }
        return services.AddOrleans(siloBuilder =>
                                   {
                                       // siloBuilder.Configure<StreamingOptions>(streamingOptions =>
                                       //                                         {
                                       //                                             streamingOptions.MinimumStreamingPeriod = options.MinimumStreamingPeriod;
                                       //                                             streamingOptions.RefreshStreamingListPeriod = options.RefreshStreamingListPeriod;
                                       //                                             streamingOptions.InitializationTimeout = options.InitializationTimeout;
                                       //                                         });
                                       siloBuilder.AddStreaming();
                                   });
    }
}
