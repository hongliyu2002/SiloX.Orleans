﻿using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using OpenTelemetry.Trace;

namespace SiloX.Orleans.Persistence.Redis.Contributors;

internal sealed class TracerProviderContributor : ITracerProviderContributor
{
    /// <inheritdoc />
    public void Configure(TracerProviderBuilder builder, IServiceConfigurationContext context)
    {
        // var persistenceOptions = context.Services.GetOptions<RedisPersistenceOptions>();
        // foreach (var storage in persistenceOptions.Storages)
        // {
        //     builder.AddRedisInstrumentation();
        // }
    }
}
