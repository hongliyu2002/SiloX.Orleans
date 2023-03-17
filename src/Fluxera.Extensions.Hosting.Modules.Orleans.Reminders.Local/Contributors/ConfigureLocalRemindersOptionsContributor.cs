using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminders.Local.Contributors;

internal sealed class ConfigureLocalRemindersOptionsContributor : ConfigureOptionsContributorBase<LocalRemindersOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Reminders:Local";
}
