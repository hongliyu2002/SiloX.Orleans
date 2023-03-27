using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SiloX.Orleans.Clustering.AdoNet.Contributors;

internal sealed class AdoNetClusteringHealthChecksContributor : IHealthChecksContributor
{
    /// <inheritdoc />
    public void ConfigureHealthChecks(IHealthChecksBuilder builder, IServiceConfigurationContext context)
    {
        var adoNetOptions = context.Services.GetOptions<AdoNetClusteringOptions>();
        adoNetOptions.ConnectionStrings = context.Services.GetOptions<ConnectionStrings>();
        if (adoNetOptions.ConnectionStrings.TryGetValue(adoNetOptions.ProviderName, out var connectionString))
        {
            switch (adoNetOptions.DbProvider)
            {
                case AdoNetDbProvider.SQLServer:
                    builder.AddSqlServer(connectionString, "SELECT 1;", adoNetOptions.ProviderName, HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                    break;
                case AdoNetDbProvider.PostgreSQL:
                    builder.AddNpgSql(connectionString, "SELECT 1;", null, adoNetOptions.ProviderName, HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                    break;
                case AdoNetDbProvider.MySQL:
                    builder.AddMySql(connectionString, adoNetOptions.ProviderName, HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                    break;
                case AdoNetDbProvider.Oracle:
                    builder.AddOracle(connectionString, "select * from v$version", adoNetOptions.ProviderName, HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                    break;
            }
        }
    }
}
