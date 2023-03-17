using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

namespace Fluxera.Extensions.Hosting.Modules.Orleans;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansServer(this IServiceCollection services, OrleansServerOptions options)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       siloBuilder.Configure<EndpointOptions>(endpoint =>
                                                                              {
                                                                                  endpoint.AdvertisedIPAddress = options.AdvertisedIPAddress;
                                                                                  endpoint.SiloPort = options.SiloPort;
                                                                                  endpoint.GatewayPort = options.GatewayPort;
                                                                                  endpoint.SiloListeningEndpoint = options.SiloListeningEndpoint;
                                                                                  endpoint.GatewayListeningEndpoint = options.GatewayListeningEndpoint;
                                                                              });
                                       siloBuilder.Configure<SiloOptions>(silo => silo.SiloName = options.SiloName);
                                   });
    }
}
