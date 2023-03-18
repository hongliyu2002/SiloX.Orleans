using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.AdoNet;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///  </summary>
    public const string DbProviderDoesNotSupport = "Database provider does not support.";

    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansAdoNetPersistence(this IServiceCollection services, AdoNetPersistenceOptions options)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       foreach (var storage in options.Storages)
                                       {
                                           if (options.ConnectionStrings.TryGetValue(storage.ConnectionStringName, out var connectionString))
                                           {
                                               siloBuilder.AddAdoNetGrainStorage(storage.ConnectionStringName,
                                                                                 persistence =>
                                                                                 {
                                                                                     persistence.ConnectionString = connectionString;
                                                                                     persistence.Invariant = storage.DbProvider switch
                                                                                                             {
                                                                                                                 AdoNetDbProvider.SQLServer => AdoNetInvariants.InvariantNameSqlServer,
                                                                                                                 AdoNetDbProvider.PostgreSQL => AdoNetInvariants.InvariantNamePostgreSql,
                                                                                                                 AdoNetDbProvider.MySQL => AdoNetInvariants.InvariantNameMySql,
                                                                                                                 AdoNetDbProvider.Oracle => AdoNetInvariants.InvariantNameOracleDatabase,
                                                                                                                 _ => throw new ArgumentOutOfRangeException(nameof(storage.DbProvider), DbProviderDoesNotSupport)
                                                                                                             };
                                                                                     persistence.InitStage = storage.InitStage;
                                                                                 });
                                           }
                                       }
                                   });
    }
}
