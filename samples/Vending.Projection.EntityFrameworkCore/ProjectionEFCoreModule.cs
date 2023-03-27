using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using JetBrains.Annotations;
using SiloX.Orleans.Clustering.AdoNet.Contributors;
using Vending.Projection.EntityFrameworkCore.Contributors;

namespace Vending.Projection.EntityFrameworkCore;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<ConfigurationModule>]
public class ProjectionEFCoreModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureProjectionEFCoreOptionsContributor>();
        context.Services.AddHealthCheckContributor<ProjectionEFCoreHealthChecksContributor>();
        context.Services.AddTracerProviderContributor<TracerProviderContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var efCoreOptions = context.Services.GetOptions<ProjectionEFCoreOptions>();
        context.Log("AddProjectionEFCore", services => services.AddProjectionEFCore(efCoreOptions));
    }
}
