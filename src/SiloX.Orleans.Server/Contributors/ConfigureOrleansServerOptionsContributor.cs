using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace SiloX.Orleans.Contributors;

internal sealed class ConfigureOrleansServerOptionsContributor : ConfigureOptionsContributorBase<OrleansServerOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Server";
}
