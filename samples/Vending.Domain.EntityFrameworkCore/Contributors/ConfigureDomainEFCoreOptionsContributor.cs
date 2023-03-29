using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Vending.Domain.EntityFrameworkCore.Contributors;

internal sealed class ConfigureDomainEFCoreOptionsContributor : ConfigureOptionsContributorBase<DomainEFCoreOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Vending:Domain:EFCore";

    /// <inheritdoc />
    protected override void AdditionalConfigure(IServiceConfigurationContext context, DomainEFCoreOptions createdOptions)
    {
        createdOptions.ConnectionStrings = context.Services.GetOptions<ConnectionStrings>();
        context.Log("Configure(DomainEFCoreOptions)", services =>
                                                          {
                                                              services.Configure<DomainEFCoreOptions>(efCore =>
                                                                                                          {
                                                                                                              efCore.ConnectionStrings = createdOptions.ConnectionStrings;
                                                                                                          });
                                                          });
    }
}
