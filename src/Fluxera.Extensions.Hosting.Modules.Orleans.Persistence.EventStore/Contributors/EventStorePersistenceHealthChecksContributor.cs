using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.EventStore.Contributors;

internal sealed class EventStorePersistenceHealthChecksContributor : IHealthChecksContributor
{
    /// <inheritdoc />
    public void ConfigureHealthChecks(IHealthChecksBuilder builder, IServiceConfigurationContext context)
    {
        var persistenceOptions = context.Services.GetObject<EventStorePersistenceOptions>();
        persistenceOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        foreach (var storage in persistenceOptions.Storages)
        {
            if (persistenceOptions.ConnectionStrings.TryGetValue(storage.ConnectionStringName, out var connectionString))
            {
                builder.AddEventStore(connectionString, $"EventStorePersistence-{storage.ConnectionStringName}", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
            }
        }
    }
}
