using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SiloX.Orleans.Reminders.AdoNet;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    public const string DbProviderDoesNotSupport = "Database provider does not support.";

    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="adoNetOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansAdoNetReminders(this IServiceCollection services, AdoNetRemindersOptions adoNetOptions)
    {
        if (adoNetOptions.ConnectionStrings.TryGetValue(adoNetOptions.ProviderName, out var connectionString))
        {
            return services.AddOrleans(siloBuilder =>
                                       {
                                           siloBuilder.UseAdoNetReminderService(reminders =>
                                                                                {
                                                                                    reminders.ConnectionString = connectionString;
                                                                                    reminders.Invariant = adoNetOptions.DbProvider switch
                                                                                                          {
                                                                                                              AdoNetDbProvider.SQLServer => AdoNetInvariants.InvariantNameSqlServerDotnetCore,
                                                                                                              AdoNetDbProvider.PostgreSQL => AdoNetInvariants.InvariantNamePostgreSql,
                                                                                                              AdoNetDbProvider.MySQL => AdoNetInvariants.InvariantNameMySql,
                                                                                                              AdoNetDbProvider.Oracle => AdoNetInvariants.InvariantNameOracleDatabase,
                                                                                                              _ => throw new ArgumentOutOfRangeException(nameof(adoNetOptions.DbProvider), DbProviderDoesNotSupport)
                                                                                                          };
                                                                                });
                                       });
        }
        return services;
    }
}
