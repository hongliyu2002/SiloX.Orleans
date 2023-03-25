using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace SiloX.Orleans.Clustering.InMemory.Contributors;

internal sealed class ConfigureInMemoryClusteringOptionsContributor : ConfigureOptionsContributorBase<InMemoryClusteringOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Clustering:InMemory";
}
