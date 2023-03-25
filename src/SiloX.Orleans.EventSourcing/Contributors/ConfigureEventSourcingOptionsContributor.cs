using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace SiloX.Orleans.EventSourcing.Contributors;

internal sealed class ConfigureEventSourcingOptionsContributor : ConfigureOptionsContributorBase<EventSourcingOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:EventSourcing";
}
