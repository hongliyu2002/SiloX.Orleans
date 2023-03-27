using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SiloX.Orleans.Streaming.EventStore.Contributors;

internal sealed class ConfigureEventStoreStreamingOptionsContributor : ConfigureOptionsContributorBase<EventStoreStreamingOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Streaming:EventStore";

    /// <inheritdoc />
    protected override void AdditionalConfigure(IServiceConfigurationContext context, EventStoreStreamingOptions createdOptions)
    {
        createdOptions.ConnectionStrings = context.Services.GetOptions<ConnectionStrings>();
        context.Log("Configure(EventStoreStreamingOptions)",
                    services =>
                    {
                        services.Configure<EventStoreStreamingOptions>(streaming =>
                                                                       {
                                                                           streaming.ConnectionStrings = createdOptions.ConnectionStrings;
                                                                       });
                    });
    }
}
