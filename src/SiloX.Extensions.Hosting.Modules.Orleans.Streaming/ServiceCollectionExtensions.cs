using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SiloX.Extensions.Hosting.Modules.Orleans.Streaming;

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
                                       // siloBuilder.Configure<StreamingOptions>(streaming =>
                                       //                                         {
                                       //                                         });
                                       siloBuilder.AddStreaming();
                                   });
    }
}
