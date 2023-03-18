using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminders.Dev.Contributors;

internal sealed class ConfigureDevRemindersOptionsContributor : ConfigureOptionsContributorBase<DevRemindersOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Reminders:Dev";
}
