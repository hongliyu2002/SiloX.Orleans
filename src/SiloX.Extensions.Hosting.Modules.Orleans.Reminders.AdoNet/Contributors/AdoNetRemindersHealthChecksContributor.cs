using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SiloX.Extensions.Hosting.Modules.Orleans.Reminders.AdoNet.Contributors;

internal sealed class AdoNetRemindersHealthChecksContributor : IHealthChecksContributor
{
    /// <inheritdoc />
    public void ConfigureHealthChecks(IHealthChecksBuilder builder, IServiceConfigurationContext context)
    {
        var remindersOptions = context.Services.GetObject<AdoNetRemindersOptions>();
        remindersOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        if (remindersOptions.ConnectionStrings.TryGetValue(remindersOptions.ProviderName, out var connectionString))
        {
            switch (remindersOptions.DbProvider)
            {
                case AdoNetDbProvider.SQLServer:
                    builder.AddSqlServer(connectionString, "SELECT 1;", "AdoNetReminders", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                    break;
                case AdoNetDbProvider.PostgreSQL:
                    builder.AddNpgSql(connectionString, "SELECT 1;", null, "AdoNetReminders", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                    break;
                case AdoNetDbProvider.MySQL:
                    builder.AddMySql(connectionString, "AdoNetReminders", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                    break;
                case AdoNetDbProvider.Oracle:
                    builder.AddOracle(connectionString, "select * from v$version", "AdoNetReminders", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                    break;
            }
        }
    }
}
