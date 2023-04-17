using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SiloX.Orleans.Clustering.AdoNet;

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
    /// <param name="options"></param>
    /// <param name="adoNetOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansAdoNetClustering(this IServiceCollection services, ClusteringOptions options, AdoNetClusteringOptions adoNetOptions)
    {
        if (adoNetOptions.ConnectionStrings.TryGetValue(adoNetOptions.ProviderName, out var connectionString))
        {
            if (options.UsedByClient)
            {
                return services.AddOrleansClient(clientBuilder =>
                                                 {
                                                     clientBuilder.UseAdoNetClustering(clustering =>
                                                                                       {
                                                                                           clustering.ConnectionString = connectionString;
                                                                                           clustering.Invariant = adoNetOptions.DbProvider switch
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
            return services.AddOrleans(siloBuilder =>
                                       {
                                           siloBuilder.UseAdoNetClustering(clustering =>
                                                                           {
                                                                               clustering.ConnectionString = connectionString;
                                                                               clustering.Invariant = adoNetOptions.DbProvider switch
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
