using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        return services.AddOrleans(siloBuilder =>
                                   {
                                       siloBuilder.Configure<ReminderOptions>(reminderOptions =>
                                                                              {
                                                                                  reminderOptions.MinimumReminderPeriod = options.MinimumReminderPeriod;
                                                                                  reminderOptions.RefreshReminderListPeriod = options.RefreshReminderListPeriod;
                                                                                  reminderOptions.InitializationTimeout = options.InitializationTimeout;
                                                                              });
                                       siloBuilder.AddReminders();
                                   });
    }
}
