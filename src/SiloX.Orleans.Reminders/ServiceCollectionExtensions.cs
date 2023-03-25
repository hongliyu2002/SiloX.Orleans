using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SiloX.Orleans.Reminders;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansReminders(this IServiceCollection services, RemindersOptions options)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       siloBuilder.Configure<RemindersOptions>(reminders =>
                                                                               {
                                                                                   reminders.MinimumRemindersPeriod = options.MinimumRemindersPeriod;
                                                                                   reminders.RefreshRemindersListPeriod = options.RefreshRemindersListPeriod;
                                                                                   reminders.InitializationTimeout = options.InitializationTimeout;
                                                                               });
                                       siloBuilder.AddReminders();
                                   });
    }
}
