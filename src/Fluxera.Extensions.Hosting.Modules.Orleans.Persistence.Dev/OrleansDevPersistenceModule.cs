using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.Dev.Contributors;
using JetBrains.Annotations;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.Dev;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<OrleansPersistenceModule>]
[DependsOn<ConfigurationModule>]
public class OrleansDevPersistenceModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureDevPersistenceOptionsContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var persistenceOptions = context.Services.GetObject<DevPersistenceOptions>();
        context.Log("AddOrleansDevPersistence", services => services.AddOrleansDevPersistence(persistenceOptions));
    }
}
