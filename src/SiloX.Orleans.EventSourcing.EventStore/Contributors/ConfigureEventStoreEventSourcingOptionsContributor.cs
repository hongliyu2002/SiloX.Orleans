using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SiloX.Orleans.EventSourcing.EventStore.Contributors;

internal sealed class ConfigureEventStoreEventSourcingOptionsContributor : ConfigureOptionsContributorBase<EventStoreEventSourcingOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:EventSourcing:EventStore";

    /// <inheritdoc />
    protected override void AdditionalConfigure(IServiceConfigurationContext context, EventStoreEventSourcingOptions createdOptions)
    {
        createdOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        context.Log("Configure(EventStoreEventSourcingOptions)",
                    services =>
                    {
                        services.Configure<EventStoreEventSourcingOptions>(eventSourcing =>
                                                                           {
                                                                               eventSourcing.ConnectionStrings = createdOptions.ConnectionStrings;
                                                                           });
                    });
    }
}
