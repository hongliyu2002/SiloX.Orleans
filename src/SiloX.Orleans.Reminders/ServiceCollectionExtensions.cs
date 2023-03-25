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
                                       siloBuilder.Configure<RemindersOptions>(remindersOptions =>
                                                                               {
                                                                                   remindersOptions.MinimumRemindersPeriod = options.MinimumRemindersPeriod;
                                                                                   remindersOptions.RefreshRemindersListPeriod = options.RefreshRemindersListPeriod;
                                                                                   remindersOptions.InitializationTimeout = options.InitializationTimeout;
                                                                               });
                                       siloBuilder.AddReminders();
                                   });
    }
}
