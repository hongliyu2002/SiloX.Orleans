using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using OpenTelemetry.Trace;

namespace SiloX.Orleans.EventSourcing.EventStore.Contributors;

internal sealed class TracerProviderContributor : ITracerProviderContributor
{
    /// <inheritdoc />
    public void Configure(TracerProviderBuilder builder, IServiceConfigurationContext context)
    {
        // var eventSourcingOptions = context.Services.GetOptions<EventStoreEventSourcingOptions>();
        // foreach (var storage in eventSourcingOptions.LogConsistencies)
        // {
        //     builder.AddEventStoreInstrumentation();
        // }
    }
}
