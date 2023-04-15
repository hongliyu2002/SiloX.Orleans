using Vending.Apps.Blazor.Services;

namespace Vending.Apps.Blazor;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddVendingBlazor(this IServiceCollection services, AppOptions options)
    {
        return services.AddSingleton<IClusterClientReady, ClusterClientReady>();
    }
}