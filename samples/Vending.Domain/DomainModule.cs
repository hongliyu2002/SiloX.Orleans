using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using SiloX.Orleans.Streaming;
using Vending.Domain.Contributors;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<StreamingModule>]
[DependsOn<DomainEFCoreModule>]
[DependsOn<ConfigurationModule>]
public class DomainModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureDomainOptionsContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var options = context.Services.GetOptions<DomainOptions>();
        context.Log("AddDomain", services => services.AddDomain(options));
    }
}
