using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace SiloX.Orleans.Reminders.Redis;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="redisOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansRedisReminders(this IServiceCollection services, RedisRemindersOptions redisOptions)
    {
        if (redisOptions.ConnectionStrings.TryGetValue(redisOptions.ProviderName, out var connectionString))
        {
            return services.AddOrleans(siloBuilder =>
                                       {
                                           siloBuilder.UseRedisReminderService(reminders =>
                                                                               {
                                                                                   reminders.ConfigurationOptions = ConfigurationOptions.Parse(connectionString);
                                                                               });
                                       });
        }
        return services;
    }
}
