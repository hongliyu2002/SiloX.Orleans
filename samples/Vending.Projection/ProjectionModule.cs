using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using SiloX.Orleans.Streaming;
using Vending.Projection.Contributors;
using Vending.Projection.EntityFrameworkCore;

namespace Vending.Projection;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<StreamingModule>]
[DependsOn<ProjectionEFCoreModule>]
[DependsOn<ConfigurationModule>]
public class ProjectionModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureProjectionOptionsContributor>();
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        var options = context.Services.GetOptions<ProjectionOptions>();
        context.Log("AddProjection", services => services.AddProjection(options));
    }
}
