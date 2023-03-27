using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using Vending.Projection.Contributors;

namespace Vending.Projection;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<ConfigurationModule>]
public class ProjectionModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureProjectionOptionsContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var options = context.Services.GetOptions<ProjectionOptions>();
        context.Log("AddProjection", services => services.AddProjection(options));
    }
}
