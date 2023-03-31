using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace SiloX.Orleans.Contributors;

internal sealed class ConfigureClientOptionsContributor : ConfigureOptionsContributorBase<ClientOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Client";
}
