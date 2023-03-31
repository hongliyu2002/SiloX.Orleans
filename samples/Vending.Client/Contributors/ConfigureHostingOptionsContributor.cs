using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Vending.Client.Contributors;

internal sealed class ConfigureClientOptionsContributor : ConfigureOptionsContributorBase<ClientOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Vending:Client";
}
