using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.InMemory.Contributors;

internal sealed class ConfigureInMemoryPersistenceOptionsContributor : ConfigureOptionsContributorBase<InMemoryPersistenceOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Persistence:InMemory";
}
