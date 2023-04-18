using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Runtime.Development;

namespace SiloX.Orleans.Streaming;

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
                                                 foreach (var broadcast in options.Broadcasts)
                                                 {
                                                     clientBuilder.AddBroadcastChannel(broadcast.ProviderName,
                                                                                       channel =>
                                                                                       {
                                                                                           channel.FireAndForgetDelivery = broadcast.FireAndForgetDelivery;
                                                                                       });
                                                 }
                                             });
        }
        return services.AddOrleans(siloBuilder =>
                                   {
                                       siloBuilder.AddStreaming();
                                       siloBuilder.UseInMemoryLeaseProvider();
                                       foreach (var broadcast in options.Broadcasts)
                                       {
                                           siloBuilder.AddBroadcastChannel(broadcast.ProviderName,
                                                                           channel =>
                                                                           {
                                                                               channel.FireAndForgetDelivery = broadcast.FireAndForgetDelivery;
                                                                           });
                                       }
                                   });
    }
}
