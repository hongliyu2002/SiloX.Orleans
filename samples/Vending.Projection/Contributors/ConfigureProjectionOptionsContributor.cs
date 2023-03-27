using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Vending.Projection.Contributors;

internal sealed class ConfigureProjectionOptionsContributor : ConfigureOptionsContributorBase<ProjectionOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Vending:Projection";
}
