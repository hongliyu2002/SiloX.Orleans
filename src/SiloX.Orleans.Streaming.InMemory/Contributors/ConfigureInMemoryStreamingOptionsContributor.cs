using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace SiloX.Orleans.Streaming.InMemory.Contributors;

internal sealed class ConfigureInMemoryStreamingOptionsContributor : ConfigureOptionsContributorBase<InMemoryStreamingOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Streaming:InMemory";
}
