using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.AdoNet.Contributors;

internal sealed class AdoNetPersistenceHealthChecksContributor : IHealthChecksContributor
{
    /// <inheritdoc />
    public void ConfigureHealthChecks(IHealthChecksBuilder builder, IServiceConfigurationContext context)
    {
        var persistenceOptions = context.Services.GetObject<AdoNetPersistenceOptions>();
        persistenceOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        foreach (var storage in persistenceOptions.Storages)
        {
            if (persistenceOptions.ConnectionStrings.TryGetValue(storage.ConnectionStringName, out var connectionString))
            {
                switch (storage.DbProvider)
                {
                    case AdoNetDbProvider.SQLServer:
                        builder.AddSqlServer(connectionString, "SELECT 1;", null, $"AdoNetPersistence-{storage.ConnectionStringName}", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                        break;
                    case AdoNetDbProvider.PostgreSQL:
                        builder.AddNpgSql(connectionString, "SELECT 1;", null, $"AdoNetPersistence-{storage.ConnectionStringName}", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                        break;
                    case AdoNetDbProvider.MySQL:
                        builder.AddMySql(connectionString, $"AdoNetPersistence-{storage.ConnectionStringName}", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                        break;
                    case AdoNetDbProvider.Oracle:
                        builder.AddOracle(connectionString, "select * from v$version", null, $"AdoNetPersistence-{storage.ConnectionStringName}", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                        break;
                }
            }
        }
    }
}
