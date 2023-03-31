using Microsoft.Extensions.DependencyInjection;

namespace Vending.Client;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddVendingClient(this IServiceCollection services, ClientOptions options)
    {
        return services;
    }
}
