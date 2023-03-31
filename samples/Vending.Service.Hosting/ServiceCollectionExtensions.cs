namespace Vending.Hosting;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddVendingHosting(this IServiceCollection services, HostingOptions options)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       siloBuilder.UseDashboard(dashboard =>
                                                                {
                                                                    dashboard.BasePath = options.Dashboard.BasePath;
                                                                    dashboard.ScriptPath = options.Dashboard.ScriptPath;
                                                                    dashboard.CustomCssPath = options.Dashboard.CustomCssPath;
                                                                    dashboard.Username = options.Dashboard.Username;
                                                                    dashboard.Password = options.Dashboard.Password;
                                                                    dashboard.Host = options.Dashboard.Host;
                                                                    dashboard.HideTrace = options.Dashboard.HideTrace;
                                                                    dashboard.HostSelf = options.Dashboard.HostSelf;
                                                                    dashboard.CounterUpdateIntervalMs = options.Dashboard.CounterUpdateIntervalMs;
                                                                    dashboard.HistoryLength = options.Dashboard.HistoryLength;
                                                                    dashboard.Port = options.Dashboard.Port;
                                                                });
                                   });
    }
}
