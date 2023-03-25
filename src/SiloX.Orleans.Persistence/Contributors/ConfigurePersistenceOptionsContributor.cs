using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace SiloX.Orleans.Persistence.Contributors;

internal sealed class ConfigurePersistenceOptionsContributor : ConfigureOptionsContributorBase<PersistenceOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Persistence";
}
