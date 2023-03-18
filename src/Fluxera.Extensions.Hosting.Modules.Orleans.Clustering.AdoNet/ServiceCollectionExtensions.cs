using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Clustering.AdoNet;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///  </summary>
    public const string DbProviderNotSupports = "Database provider does not support.";

    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansAdoNetClustering(this IServiceCollection services, AdoNetClusteringOptions options)
    {
        if (!options.ConnectionStrings.TryGetValue(options.ConnectionStringName, out var connectionString))
        {
            return services;
        }
        if (options.UsedByClient)
        {
            return services.AddOrleansClient(clientBuilder =>
                                             {
                                                 clientBuilder.UseAdoNetClustering(clustering =>
                                                                                   {
                                                                                       clustering.ConnectionString = connectionString;
                                                                                       clustering.Invariant = options.DbProvider switch
                                                                                                              {
                                                                                                                  AdoNetDbProvider.SQLServer => AdoNetInvariants.InvariantNameSqlServer,
                                                                                                                  AdoNetDbProvider.PostgreSQL => AdoNetInvariants.InvariantNamePostgreSql,
                                                                                                                  AdoNetDbProvider.MySQL => AdoNetInvariants.InvariantNameMySql,
                                                                                                                  AdoNetDbProvider.Oracle => AdoNetInvariants.InvariantNameOracleDatabase,
                                                                                                                  _ => throw new ArgumentOutOfRangeException(nameof(options.DbProvider), DbProviderNotSupports)
                                                                                                              };
                                                                                   });
                                             });
        }
        return services.AddOrleans(siloBuilder =>
                                   {
                                       siloBuilder.UseAdoNetClustering(clustering =>
                                                                       {
                                                                           clustering.ConnectionString = connectionString;
                                                                           clustering.Invariant = options.DbProvider switch
                                                                                                  {
                                                                                                      AdoNetDbProvider.SQLServer => AdoNetInvariants.InvariantNameSqlServer,
                                                                                                      AdoNetDbProvider.PostgreSQL => AdoNetInvariants.InvariantNamePostgreSql,
                                                                                                      AdoNetDbProvider.MySQL => AdoNetInvariants.InvariantNameMySql,
                                                                                                      AdoNetDbProvider.Oracle => AdoNetInvariants.InvariantNameOracleDatabase,
                                                                                                      _ => throw new ArgumentOutOfRangeException(nameof(options.DbProvider), DbProviderNotSupports)
                                                                                                  };
                                                                       });
                                   });
    }
}
