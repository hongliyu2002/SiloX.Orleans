using Microsoft.Extensions.DependencyInjection;

namespace SiloX.Orleans.Persistence;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansPersistence(this IServiceCollection services, PersistenceOptions options)
    {
        // return services.AddOrleans(siloBuilder =>
        //                            {
        //                            });
        return services;
    }
}
