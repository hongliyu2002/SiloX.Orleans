using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using SiloX.Extensions.Hosting.Modules.Orleans.Contributors;
using JetBrains.Annotations;

namespace SiloX.Extensions.Hosting.Modules.Orleans;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<ConfigurationModule>]
public class OrleansServerModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureOrleansServerOptionsContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var serverOptions = context.Services.GetOptions<OrleansServerOptions>();
        context.Log("AddOrleansServer", services => services.AddOrleansServer(serverOptions));
    }
}
