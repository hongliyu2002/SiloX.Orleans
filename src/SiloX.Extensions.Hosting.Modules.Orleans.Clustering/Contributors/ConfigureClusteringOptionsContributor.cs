using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace SiloX.Extensions.Hosting.Modules.Orleans.Clustering.Contributors;

internal sealed class ConfigureClusteringOptionsContributor : ConfigureOptionsContributorBase<ClusteringOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Clustering";
}
