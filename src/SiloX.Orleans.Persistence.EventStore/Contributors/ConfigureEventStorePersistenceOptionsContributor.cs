using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SiloX.Orleans.Persistence.EventStore.Contributors;

internal sealed class ConfigureEventStorePersistenceOptionsContributor : ConfigureOptionsContributorBase<EventStorePersistenceOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Persistence:EventStore";

    /// <inheritdoc />
    protected override void AdditionalConfigure(IServiceConfigurationContext context, EventStorePersistenceOptions createdOptions)
    {
        createdOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        context.Log("Configure(EventStorePersistenceOptions)",
                    services =>
                    {
                        services.Configure<EventStorePersistenceOptions>(persistence =>
                                                                         {
                                                                             persistence.ConnectionStrings = createdOptions.ConnectionStrings;
                                                                         });
                    });
    }
}
