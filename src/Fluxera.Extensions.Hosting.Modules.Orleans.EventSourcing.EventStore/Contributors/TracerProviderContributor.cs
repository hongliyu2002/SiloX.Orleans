using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using OpenTelemetry.Trace;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.EventSourcing.EventStore.Contributors;

internal sealed class TracerProviderContributor : ITracerProviderContributor
{
    /// <inheritdoc />
    public void Configure(TracerProviderBuilder builder, IServiceConfigurationContext context)
    {
        // var eventSourcingOptions = context.Services.GetObject<EventStoreEventSourcingOptions>();
        // foreach (var storage in eventSourcingOptions.LogConsistencyOptions)
        // {
        //     builder.AddEventStoreInstrumentation();
        // }
    }
}
