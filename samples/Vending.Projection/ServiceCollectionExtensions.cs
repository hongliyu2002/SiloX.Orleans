using Microsoft.Extensions.DependencyInjection;

namespace Vending.Projection;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddProjection(this IServiceCollection services, ProjectionOptions options)
    {
        return services;
    }
}
