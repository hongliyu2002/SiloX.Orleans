using Microsoft.Extensions.DependencyInjection;

namespace Vending.App.Wpf;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddVendingApp(this IServiceCollection services, AppOptions options)
    {
        return services;
    }
}
