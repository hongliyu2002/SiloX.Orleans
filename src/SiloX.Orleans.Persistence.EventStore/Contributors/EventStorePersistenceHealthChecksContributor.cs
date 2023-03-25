using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SiloX.Orleans.Persistence.EventStore.Contributors;

internal sealed class EventStorePersistenceHealthChecksContributor : IHealthChecksContributor
{
    /// <inheritdoc />
    public void ConfigureHealthChecks(IHealthChecksBuilder builder, IServiceConfigurationContext context)
    {
        var eventStoreOptions = context.Services.GetObject<EventStorePersistenceOptions>();
        eventStoreOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        foreach (var storage in eventStoreOptions.Storages)
        {
            if (eventStoreOptions.ConnectionStrings.TryGetValue(storage.ProviderName, out var connectionString))
            {
                builder.AddEventStore(connectionString, storage.ProviderName, HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
            }
        }
    }
}
