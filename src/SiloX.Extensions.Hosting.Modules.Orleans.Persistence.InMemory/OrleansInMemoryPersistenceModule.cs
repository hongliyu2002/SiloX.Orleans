using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using SiloX.Extensions.Hosting.Modules.Orleans.Persistence.InMemory.Contributors;
using JetBrains.Annotations;

namespace SiloX.Extensions.Hosting.Modules.Orleans.Persistence.InMemory;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<OrleansPersistenceModule>]
[DependsOn<ConfigurationModule>]
public class OrleansInMemoryPersistenceModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureInMemoryPersistenceOptionsContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var persistenceOptions = context.Services.GetObject<InMemoryPersistenceOptions>();
        context.Log("AddOrleansInMemoryPersistence", services => services.AddOrleansInMemoryPersistence(persistenceOptions));
    }
}
