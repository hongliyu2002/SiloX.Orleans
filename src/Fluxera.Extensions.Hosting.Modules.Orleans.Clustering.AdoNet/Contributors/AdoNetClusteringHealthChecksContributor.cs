using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Clustering.AdoNet.Contributors;

internal sealed class AdoNetClusteringHealthChecksContributor : IHealthChecksContributor
{
    /// <inheritdoc />
    public void ConfigureHealthChecks(IHealthChecksBuilder builder, IServiceConfigurationContext context)
    {
        var clusteringOptions = context.Services.GetObject<AdoNetClusteringOptions>();
        clusteringOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        if (!clusteringOptions.ConnectionStrings.TryGetValue(clusteringOptions.ConnectionStringName, out var connectionString))
        {
            return;
        }
        switch (clusteringOptions.DbProvider)
        {
            case AdoNetDbProvider.SQLServer:
                builder.AddSqlServer(connectionString, "SELECT 1;", null, "AdoNetClustering", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                break;
            case AdoNetDbProvider.PostgreSQL:
                builder.AddNpgSql(connectionString, "SELECT 1;", null, "AdoNetClustering", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                break;
            case AdoNetDbProvider.MySQL:
                builder.AddMySql(connectionString, "AdoNetClustering", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                break;
            case AdoNetDbProvider.Oracle:
                builder.AddOracle(connectionString, "select * from v$version", null, "AdoNetClustering", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
                break;
        }
    }
}
