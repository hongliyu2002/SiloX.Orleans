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
        var streamingOptions = context.Services.GetObject<EventStoreStreamingOptions>();
        streamingOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        foreach (var streams in streamingOptions.StreamsOptions)
        {
            if (streamingOptions.ConnectionStrings.TryGetValue(streams.ProviderName, out var connectionString))
            {
                builder.AddEventStore(connectionString, $"EventStoreStreaming-{streams.ProviderName}", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
            }
        }
    }
}
