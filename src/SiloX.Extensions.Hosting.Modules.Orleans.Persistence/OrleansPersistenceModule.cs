using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using SiloX.Extensions.Hosting.Modules.Orleans.Persistence.Contributors;
using JetBrains.Annotations;

namespace SiloX.Extensions.Hosting.Modules.Orleans.Persistence;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<ConfigurationModule>]
public class OrleansPersistenceModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigurePersistenceOptionsContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var persistenceOptions = context.Services.GetOptions<PersistenceOptions>();
        context.Log("AddOrleansPersistence", services => services.AddOrleansPersistence(persistenceOptions));
    }
}
