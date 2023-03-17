using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminders.Local;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansLocalReminders(this IServiceCollection services, LocalRemindersOptions options)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       siloBuilder.UseInMemoryReminderService();
                                   });
    }
}
