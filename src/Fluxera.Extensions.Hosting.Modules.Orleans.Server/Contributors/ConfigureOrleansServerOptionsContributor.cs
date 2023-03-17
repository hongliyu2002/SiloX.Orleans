using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Contributors;

internal sealed class ConfigureOrleansServerOptionsContributor : ConfigureOptionsContributorBase<OrleansServerOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Server";
}
