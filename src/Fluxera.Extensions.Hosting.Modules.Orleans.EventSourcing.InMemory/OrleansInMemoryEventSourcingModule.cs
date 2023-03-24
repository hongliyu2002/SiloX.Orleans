using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.Orleans.EventSourcing.InMemory.Contributors;
using JetBrains.Annotations;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.EventSourcing.InMemory;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<OrleansEventSourcingModule>]
[DependsOn<ConfigurationModule>]
public class OrleansInMemoryEventSourcingModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureInMemoryEventSourcingOptionsContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var eventSourcingOptions = context.Services.GetObject<InMemoryEventSourcingOptions>();
        context.Log("AddOrleansInMemoryEventSourcing", services => services.AddOrleansInMemoryEventSourcing(eventSourcingOptions));
    }
}
