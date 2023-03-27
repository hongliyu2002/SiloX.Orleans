using Microsoft.Extensions.DependencyInjection;
using Vending.Domain;

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
    public static IServiceCollection AddDomain(this IServiceCollection services, DomainOptions options)
    {
        return services;
    }
}
