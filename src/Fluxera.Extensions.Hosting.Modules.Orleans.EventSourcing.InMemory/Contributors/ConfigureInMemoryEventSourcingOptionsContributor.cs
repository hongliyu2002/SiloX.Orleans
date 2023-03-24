using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.EventSourcing.InMemory.Contributors;

internal sealed class ConfigureInMemoryEventSourcingOptionsContributor : ConfigureOptionsContributorBase<InMemoryEventSourcingOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:EventSourcing:InMemory";
}
