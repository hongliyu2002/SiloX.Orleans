﻿using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using OpenTelemetry.Trace;

namespace Vending.Projection.EntityFrameworkCore.Contributors;

internal sealed class TracerProviderContributor : ITracerProviderContributor
{
    /// <inheritdoc />
    public void Configure(TracerProviderBuilder builder, IServiceConfigurationContext context)
    {
        // var options = context.Services.GetOptions<ProjectionEFCoreOptions>();
        // builder.AddSqlClientInstrumentation();
    }
}
