using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace SiloX.Extensions.Hosting.Modules.Orleans.EventSourcing.InMemory.Contributors;

internal sealed class ConfigureInMemoryEventSourcingOptionsContributor : ConfigureOptionsContributorBase<InMemoryEventSourcingOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:EventSourcing:InMemory";
}
