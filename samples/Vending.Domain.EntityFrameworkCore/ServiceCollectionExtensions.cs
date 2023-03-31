using Fluxera.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Vending.Domain.EntityFrameworkCore;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the <see cref="DomainDbContext" /> to the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    /// <param name="efCoreOptions">The <see cref="DomainEFCoreOptions" />.</param>
    /// <returns>The <see cref="IServiceCollection" />.</returns>
    public static IServiceCollection AddDomainEFCore(this IServiceCollection services, DomainEFCoreOptions efCoreOptions)
    {
        if (efCoreOptions.ConnectionStrings.TryGetValue(efCoreOptions.ProviderName, out var connectionString))
        {
            return services.AddDbContextPool<DomainDbContext>(builder =>
                                                              {
                                                                  builder.UseSqlServer(connectionString, dbContext =>
                                                                                                         {
                                                                                                             if (efCoreOptions.MigrationsHistoryTable.IsNotNullOrEmpty())
                                                                                                             {
                                                                                                                 var schemaAndName = efCoreOptions.MigrationsHistoryTable!.Trim().Split(".");
                                                                                                                 switch (schemaAndName.Length)
                                                                                                                 {
                                                                                                                     case >= 1:
                                                                                                                         dbContext.MigrationsHistoryTable(schemaAndName[1], schemaAndName[0]);
                                                                                                                         break;
                                                                                                                     default:
                                                                                                                         dbContext.MigrationsHistoryTable(schemaAndName[0]);
                                                                                                                         break;
                                                                                                                 }
                                                                                                             }
                                                                                                             dbContext.EnableRetryOnFailure(efCoreOptions.MaxRetry, efCoreOptions.MaxRetryDelay, new[] { -999 });
                                                                                                             dbContext.UseQuerySplittingBehavior(efCoreOptions.QuerySplittingBehavior);
                                                                                                         });
                                                              });
        }
        return services;
    }
}
