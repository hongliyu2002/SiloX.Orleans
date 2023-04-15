using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Vending.Apps.Wpf.Contributors;

internal sealed class ConfigureAppOptionsContributor : ConfigureOptionsContributorBase<AppOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Vending:Apps.Wpf";
}
