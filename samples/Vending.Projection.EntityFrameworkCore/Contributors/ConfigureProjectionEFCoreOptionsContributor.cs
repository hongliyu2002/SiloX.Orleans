using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Vending.Projection.EntityFrameworkCore.Contributors;

internal sealed class ConfigureProjectionEFCoreOptionsContributor : ConfigureOptionsContributorBase<ProjectionEFCoreOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Vending:Projection:EFCore";

    /// <inheritdoc />
    protected override void AdditionalConfigure(IServiceConfigurationContext context, ProjectionEFCoreOptions createdOptions)
    {
        createdOptions.ConnectionStrings = context.Services.GetOptions<ConnectionStrings>();
        context.Log("Configure(ProjectionEFCoreOptions)", services =>
                                                          {
                                                              services.Configure<ProjectionEFCoreOptions>(efCore =>
                                                                                                          {
                                                                                                              efCore.ConnectionStrings = createdOptions.ConnectionStrings;
                                                                                                          });
                                                          });
    }
}
