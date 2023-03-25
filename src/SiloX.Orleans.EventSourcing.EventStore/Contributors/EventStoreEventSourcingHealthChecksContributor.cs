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
        var eventStoreOptions = context.Services.GetObject<EventStoreEventSourcingOptions>();
        eventStoreOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        foreach (var logConsistency in eventStoreOptions.LogConsistencies)
        {
            if (eventStoreOptions.ConnectionStrings.TryGetValue(logConsistency.ProviderName, out var connectionString))
            {
                builder.AddEventStore(connectionString, logConsistency.ProviderName, HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
            }
        }
    }
}
