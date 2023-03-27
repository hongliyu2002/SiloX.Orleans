using Microsoft.Extensions.DependencyInjection;

namespace Vending.Projection.EntityFrameworkCore;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddProjectionEFCore(this IServiceCollection services, ProjectionEFCoreOptions options)
    {
        return services;
    }
}
