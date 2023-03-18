using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using OpenTelemetry.Trace;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.EventStore.Contributors;

internal sealed class TracerProviderContributor : ITracerProviderContributor
{
    /// <inheritdoc />
    public void Configure(TracerProviderBuilder builder, IServiceConfigurationContext context)
    {
        // var persistenceOptions = context.Services.GetObject<EventStorePersistenceOptions>();
        // foreach (var storage in persistenceOptions.Storages)
        // {
        //     builder.AddEventStoreInstrumentation();
        // }
    }
}
