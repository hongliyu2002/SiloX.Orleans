using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SiloX.Orleans.Persistence.AdoNet;

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
    public static IServiceCollection AddOrleansAdoNetPersistence(this IServiceCollection services, AdoNetPersistenceOptions adoNetOptions)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       foreach (var storage in adoNetOptions.Storages)
                                       {
                                           if (adoNetOptions.ConnectionStrings.TryGetValue(storage.ProviderName, out var connectionString))
                                           {
                                               siloBuilder.AddAdoNetGrainStorage(storage.ProviderName,
                                                                                 persistence =>
                                                                                 {
                                                                                     persistence.ConnectionString = connectionString;
                                                                                     persistence.Invariant = storage.DbProvider switch
                                                                                                             {
                                                                                                                 AdoNetDbProvider.SQLServer => AdoNetInvariants.InvariantNameSqlServerDotnetCore,
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
