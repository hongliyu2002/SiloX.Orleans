using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SiloX.Orleans;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansClient(this IServiceCollection services, ClientOptions options)
    {
        // return services.AddOrleansClient(clientBuilder =>
        //                                  {
        //                                  });
        return services;
    }
}
