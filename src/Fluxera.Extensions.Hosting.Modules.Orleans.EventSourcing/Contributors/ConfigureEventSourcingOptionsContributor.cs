using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.EventSourcing.Contributors;

internal sealed class ConfigureEventSourcingOptionsContributor : ConfigureOptionsContributorBase<EventSourcingOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:EventSourcing";
}
