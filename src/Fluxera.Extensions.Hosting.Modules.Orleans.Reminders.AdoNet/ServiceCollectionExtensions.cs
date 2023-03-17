using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminders.AdoNet;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    private const string notSupportsMessage = "Database provider does not support.";

    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansAdoNetReminders(this IServiceCollection services, AdoNetRemindersOptions options)
    {
        if (!options.ConnectionStrings.TryGetValue(options.ConnectionStringName, out var connectionString))
        {
            return services;
        }
        return services.AddOrleans(siloBuilder =>
                                   {
                                       siloBuilder.UseAdoNetReminderService(reminders =>
                                                                            {
                                                                                reminders.ConnectionString = connectionString;
                                                                                reminders.Invariant = options.DbProvider switch
                                                                                                      {
                                                                                                          AdoNetDbProvider.SQLServer => AdoNetInvariants.InvariantNameSqlServer,
                                                                                                          AdoNetDbProvider.PostgreSQL => AdoNetInvariants.InvariantNamePostgreSql,
                                                                                                          AdoNetDbProvider.MySQL => AdoNetInvariants.InvariantNameMySql,
                                                                                                          AdoNetDbProvider.Oracle => AdoNetInvariants.InvariantNameOracleDatabase,
                                                                                                          _ => throw new ArgumentOutOfRangeException(nameof(options.DbProvider), notSupportsMessage)
                                                                                                      };
                                                                            });
                                   });
    }
}
