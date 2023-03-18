using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Clustering.Dev.Contributors;

internal sealed class ConfigureDevClusteringOptionsContributor : ConfigureOptionsContributorBase<DevClusteringOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Clustering:Dev";
}
