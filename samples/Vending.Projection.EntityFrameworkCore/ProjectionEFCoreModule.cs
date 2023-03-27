using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
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
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var options = context.Services.GetOptions<ProjectionEFCoreOptions>();
        context.Log("AddProjectionEFCore", services => services.AddProjectionEFCore(options));
    }
}
