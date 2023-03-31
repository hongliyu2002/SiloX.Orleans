using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Vending.Hosting.Contributors;

internal sealed class ConfigureHostingOptionsContributor : ConfigureOptionsContributorBase<HostingOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Vending:Hosting";
}
