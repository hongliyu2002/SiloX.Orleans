using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Vending.Domain.Contributors;

internal sealed class ConfigureDomainOptionsContributor : ConfigureOptionsContributorBase<DomainOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Vending:Domain";
}
