using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.EventSourcing.EventStore.Contributors;

internal sealed class EventStoreEventSourcingHealthChecksContributor : IHealthChecksContributor
{
    /// <inheritdoc />
    public void ConfigureHealthChecks(IHealthChecksBuilder builder, IServiceConfigurationContext context)
    {
        var eventSourcingOptions = context.Services.GetObject<EventStoreEventSourcingOptions>();
        eventSourcingOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        foreach (var storage in eventSourcingOptions.Storages)
        {
            if (eventSourcingOptions.ConnectionStrings.TryGetValue(storage.ConnectionStringName, out var connectionString))
            {
                builder.AddEventStore(connectionString, $"EventStoreEventSourcing-{storage.ConnectionStringName}", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
            }
        }
    }
}
