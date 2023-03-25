using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SiloX.Orleans.Streaming.EventStore.Contributors;

internal sealed class EventStoreStreamingHealthChecksContributor : IHealthChecksContributor
{
    /// <inheritdoc />
    public void ConfigureHealthChecks(IHealthChecksBuilder builder, IServiceConfigurationContext context)
    {
        var eventStoreOptions = context.Services.GetObject<EventStoreStreamingOptions>();
        eventStoreOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        foreach (var streams in eventStoreOptions.Streams)
        {
            if (eventStoreOptions.ConnectionStrings.TryGetValue(streams.ProviderName, out var connectionString))
            {
                builder.AddEventStore(connectionString, streams.ProviderName, HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
            }
        }
    }
}
