using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminders.Redis;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansRedisReminders(this IServiceCollection services, RedisRemindersOptions options)
    {
        if (!options.ConnectionStrings.TryGetValue(options.ConnectionStringName, out var connectionString))
        {
            return services;
        }
        return services.AddOrleans(siloBuilder =>
                                   {
                                       siloBuilder.UseRedisReminderService(reminders =>
                                                                           {
                                                                               reminders.ConfigurationOptions = ConfigurationOptions.Parse(connectionString);
                                                                           });
                                   });
    }
}
