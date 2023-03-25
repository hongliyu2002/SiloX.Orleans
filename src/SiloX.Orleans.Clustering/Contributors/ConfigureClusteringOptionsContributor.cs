using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace SiloX.Orleans.Clustering.Contributors;

internal sealed class ConfigureClusteringOptionsContributor : ConfigureOptionsContributorBase<ClusteringOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Clustering";
}
