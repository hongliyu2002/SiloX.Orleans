using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Clustering.Local.Contributors;

internal sealed class ConfigureLocalClusteringOptionsContributor : ConfigureOptionsContributorBase<LocalClusteringOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Clustering:Local";
}
