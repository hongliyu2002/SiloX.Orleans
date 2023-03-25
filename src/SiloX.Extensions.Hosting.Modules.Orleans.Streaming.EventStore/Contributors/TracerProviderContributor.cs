using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using OpenTelemetry.Trace;

namespace SiloX.Extensions.Hosting.Modules.Orleans.Streaming.EventStore.Contributors;

internal sealed class TracerProviderContributor : ITracerProviderContributor
{
    /// <inheritdoc />
    public void Configure(TracerProviderBuilder builder, IServiceConfigurationContext context)
    {
        // var streamingOptions = context.Services.GetObject<EventStoreStreamingOptions>();
        // foreach (var storage in streamingOptions.StreamsOptions)
        // {
        //     builder.AddEventStoreInstrumentation();
        // }
    }
}
