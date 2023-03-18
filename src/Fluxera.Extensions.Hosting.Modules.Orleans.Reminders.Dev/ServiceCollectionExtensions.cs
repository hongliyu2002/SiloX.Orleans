using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminders.Dev;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansDevReminders(this IServiceCollection services, DevRemindersOptions options)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       siloBuilder.UseInMemoryReminderService();
                                   });
    }
}
