using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace SiloX.Orleans.Streaming.Contributors;

internal sealed class ConfigureStreamingOptionsContributor : ConfigureOptionsContributorBase<StreamingOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Streaming";
}
