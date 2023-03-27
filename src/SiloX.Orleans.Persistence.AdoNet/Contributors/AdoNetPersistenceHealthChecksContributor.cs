using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SiloX.Orleans.Persistence.AdoNet.Contributors;

internal sealed class AdoNetPersistenceHealthChecksContributor : IHealthChecksContributor
{
    /// <inheritdoc />
    public void ConfigureHealthChecks(IHealthChecksBuilder builder, IServiceConfigurationContext context)
    {
        var adoNetOptions = context.Services.GetOptions<AdoNetPersistenceOptions>();
        adoNetOptions.ConnectionStrings = context.Services.GetOptions<ConnectionStrings>();
        foreach (var storage in adoNetOptions.Storages)
        {
            if (adoNetOptions.ConnectionStrings.TryGetValue(storage.ProviderName, out var connectionString))
            {
                switch (storage.DbProvider)
                {
                    case AdoNetDbProvider.SQLServer:
                        builder.AddSqlServer(connectionString, "SELECT 1;", storage.ProviderName, HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                        break;
                    case AdoNetDbProvider.PostgreSQL:
                        builder.AddNpgSql(connectionString, "SELECT 1;", null, storage.ProviderName, HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                        break;
                    case AdoNetDbProvider.MySQL:
                        builder.AddMySql(connectionString, storage.ProviderName, HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                        break;
                    case AdoNetDbProvider.Oracle:
                        builder.AddOracle(connectionString, "select * from v$version", storage.ProviderName, HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                        break;
                }
            }
        }
    }
}
