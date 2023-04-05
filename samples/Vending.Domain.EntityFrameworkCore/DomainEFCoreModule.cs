using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using JetBrains.Annotations;
using Vending.Domain.EntityFrameworkCore.Contributors;

namespace Vending.Domain.EntityFrameworkCore;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<ConfigurationModule>]
public class DomainEFCoreModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureDomainEFCoreOptionsContributor>();
        context.Services.AddHealthCheckContributor<DomainEFCoreHealthChecksContributor>();
        context.Services.AddTracerProviderContributor<TracerProviderContributor>();
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        var efCoreOptions = context.Services.GetOptions<DomainEFCoreOptions>();
        context.Log("AddDomainEFCore", services => services.AddDomainEFCore(efCoreOptions));
    }
}
