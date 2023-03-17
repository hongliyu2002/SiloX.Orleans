using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Clustering.Contributors;

internal sealed class ConfigureClusteringOptionsContributor : ConfigureOptionsContributorBase<ClusteringOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Clustering";
}
