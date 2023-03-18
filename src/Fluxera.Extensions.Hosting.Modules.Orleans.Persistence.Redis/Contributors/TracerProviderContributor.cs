using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using OpenTelemetry.Trace;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.Redis.Contributors;

internal sealed class TracerProviderContributor : ITracerProviderContributor
{
    /// <inheritdoc />
    public void Configure(TracerProviderBuilder builder, IServiceConfigurationContext context)
    {
        // var persistenceOptions = context.Services.GetObject<RedisPersistenceOptions>();
        // foreach (var storage in persistenceOptions.Storages)
        // {
        //     builder.AddRedisInstrumentation();
        // }
    }
}
