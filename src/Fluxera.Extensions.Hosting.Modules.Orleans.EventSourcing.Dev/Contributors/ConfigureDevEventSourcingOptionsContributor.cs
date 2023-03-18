using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.EventSourcing.Dev.Contributors;

internal sealed class ConfigureDevEventSourcingOptionsContributor : ConfigureOptionsContributorBase<DevEventSourcingOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:EventSourcing:Dev";
}
