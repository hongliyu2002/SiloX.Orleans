using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SiloX.Orleans.Clustering.AdoNet.Contributors;

internal sealed class ConfigureAdoNetClusteringOptionsContributor : ConfigureOptionsContributorBase<AdoNetClusteringOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Clustering:AdoNet";

    /// <inheritdoc />
    protected override void AdditionalConfigure(IServiceConfigurationContext context, AdoNetClusteringOptions createdOptions)
    {
        createdOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        context.Log("Configure(AdoNetClusteringOptions)",
                    services =>
                    {
                        services.Configure<AdoNetClusteringOptions>(clustering =>
                                                                    {
                                                                        clustering.ConnectionStrings = createdOptions.ConnectionStrings;
                                                                    });
                    });
    }
}
