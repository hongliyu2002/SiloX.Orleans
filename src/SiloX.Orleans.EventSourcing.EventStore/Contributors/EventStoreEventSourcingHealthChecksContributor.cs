using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SiloX.Orleans.EventSourcing.EventStore.Contributors;

internal sealed class EventStoreEventSourcingHealthChecksContributor : IHealthChecksContributor
{
    /// <inheritdoc />
    public void ConfigureHealthChecks(IHealthChecksBuilder builder, IServiceConfigurationContext context)
    {
        var eventSourcingOptions = context.Services.GetObject<EventStoreEventSourcingOptions>();
        eventSourcingOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        foreach (var logConsistency in eventSourcingOptions.LogConsistencies)
        {
            if (eventSourcingOptions.ConnectionStrings.TryGetValue(logConsistency.ProviderName, out var connectionString))
            {
                builder.AddEventStore(connectionString, $"EventStoreEventSourcing-{logConsistency.ProviderName}", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
            }
        }
    }
}
