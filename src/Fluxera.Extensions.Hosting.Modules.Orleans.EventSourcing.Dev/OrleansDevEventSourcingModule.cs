using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.Orleans.EventSourcing.Dev.Contributors;
using JetBrains.Annotations;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.EventSourcing.Dev;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<OrleansEventSourcingModule>]
[DependsOn<ConfigurationModule>]
public class OrleansDevEventSourcingModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureDevEventSourcingOptionsContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var eventSourcingOptions = context.Services.GetObject<DevEventSourcingOptions>();
        context.Log("AddOrleansDevEventSourcing", services => services.AddOrleansDevEventSourcing(eventSourcingOptions));
    }
}
