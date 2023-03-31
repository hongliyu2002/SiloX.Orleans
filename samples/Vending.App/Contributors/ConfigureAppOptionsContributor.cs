using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Vending.App.Contributors;

internal sealed class ConfigureAppOptionsContributor : ConfigureOptionsContributorBase<AppOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Vending:App";
}
