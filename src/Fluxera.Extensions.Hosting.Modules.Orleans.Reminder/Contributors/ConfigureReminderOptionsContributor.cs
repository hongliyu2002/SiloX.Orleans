using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminder.Contributors;

internal sealed class ConfigureReminderOptionsContributor : ConfigureOptionsContributorBase<ReminderOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Reminder";
}
