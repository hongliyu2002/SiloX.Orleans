using Microsoft.Extensions.DependencyInjection;
using Vending.App.Wpf.Services;

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
    public static IServiceCollection AddVendingWpf(this IServiceCollection services, AppOptions options)
    {
        return services.AddSingleton<IClusterClientReady, ClusterClientReady>();
    }
}