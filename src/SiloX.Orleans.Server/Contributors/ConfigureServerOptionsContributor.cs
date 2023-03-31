using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace SiloX.Orleans.Contributors;

internal sealed class ConfigureServerOptionsContributor : ConfigureOptionsContributorBase<ServerOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Server";
}
