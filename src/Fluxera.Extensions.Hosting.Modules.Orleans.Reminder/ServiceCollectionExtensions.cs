using Microsoft.Extensions.DependencyInjection;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminder;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansReminder(this IServiceCollection services, ReminderOptions options)
    {
        // return services.AddOrleans(siloBuilder =>
        //                            {
        //                            });
        return services;
    }
}
