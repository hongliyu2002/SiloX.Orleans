using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.Redis.Contributors;

internal sealed class RedisPersistenceHealthChecksContributor : IHealthChecksContributor
{
    /// <inheritdoc />
    public void ConfigureHealthChecks(IHealthChecksBuilder builder, IServiceConfigurationContext context)
    {
        var persistenceOptions = context.Services.GetObject<RedisPersistenceOptions>();
        persistenceOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        foreach (var storage in persistenceOptions.Storages)
        {
            if (persistenceOptions.ConnectionStrings.TryGetValue(storage.ProviderName, out var connectionString))
            {
                builder.AddRedis(connectionString, $"RedisPersistence-{storage.ProviderName}", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
            }
        }
    }
}
