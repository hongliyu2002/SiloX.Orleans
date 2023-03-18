using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.Dev.Contributors;

internal sealed class ConfigureDevPersistenceOptionsContributor : ConfigureOptionsContributorBase<DevPersistenceOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Persistence:Dev";
}
