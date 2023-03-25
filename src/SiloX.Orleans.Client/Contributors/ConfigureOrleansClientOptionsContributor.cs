using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace SiloX.Orleans.Contributors;

internal sealed class ConfigureOrleansClientOptionsContributor : ConfigureOptionsContributorBase<OrleansClientOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Client";
}
