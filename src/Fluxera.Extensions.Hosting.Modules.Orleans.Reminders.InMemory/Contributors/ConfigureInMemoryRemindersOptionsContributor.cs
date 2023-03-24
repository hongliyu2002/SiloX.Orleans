using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminders.InMemory.Contributors;

internal sealed class ConfigureInMemoryRemindersOptionsContributor : ConfigureOptionsContributorBase<InMemoryRemindersOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Reminders:InMemory";
}
