using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.Local.Contributors;

internal sealed class ConfigureLocalPersistenceOptionsContributor : ConfigureOptionsContributorBase<LocalPersistenceOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Persistence:Local";
}
