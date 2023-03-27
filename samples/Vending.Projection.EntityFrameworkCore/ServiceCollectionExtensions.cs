using Fluxera.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Vending.Projection.EntityFrameworkCore;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="efCoreOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddProjectionEFCore(this IServiceCollection services, ProjectionEFCoreOptions efCoreOptions)
    {
        if (efCoreOptions.ConnectionStrings.TryGetValue(efCoreOptions.ProviderName, out var connectionString))
        {
            return services.AddDbContextPool<ProjectionDbContext>(optionsBuilder =>
                                                                  {
                                                                      optionsBuilder.UseSqlServer(connectionString, dbContext =>
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
                                                                                                                        var retryDelay = TimeSpan.FromMilliseconds(efCoreOptions.MaxRetryDelay);
                                                                                                                        dbContext.EnableRetryOnFailure(efCoreOptions.MaxRetry, retryDelay, new[] { -999 });
                                                                                                                        dbContext.UseQuerySplittingBehavior(efCoreOptions.QuerySplittingBehavior);
                                                                                                                    });
                                                                  });
        }
        return services;
    }
}
