using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using SiloX.Orleans.Persistence.InMemory.Contributors;

namespace SiloX.Orleans.Persistence.InMemory;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<PersistenceModule>]
[DependsOn<ConfigurationModule>]
public class InMemoryPersistenceModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureInMemoryPersistenceOptionsContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var inMemoryOptions = context.Services.GetOptions<InMemoryPersistenceOptions>();
        context.Log("AddOrleansInMemoryPersistence", services => services.AddOrleansInMemoryPersistence(inMemoryOptions));
    }
}
