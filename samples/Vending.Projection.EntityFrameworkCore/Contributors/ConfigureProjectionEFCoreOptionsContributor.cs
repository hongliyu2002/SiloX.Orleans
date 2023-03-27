using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Vending.Projection.EntityFrameworkCore.Contributors;

internal sealed class ConfigureProjectionEFCoreOptionsContributor : ConfigureOptionsContributorBase<ProjectionEFCoreOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Vending:Projection:EntityFrameworkCore";
}
