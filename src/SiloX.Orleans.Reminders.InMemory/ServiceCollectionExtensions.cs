using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SiloX.Orleans.Reminders.InMemory;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="inMemoryOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansInMemoryReminders(this IServiceCollection services, InMemoryRemindersOptions inMemoryOptions)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       siloBuilder.UseInMemoryReminderService();
                                   });
    }
}
