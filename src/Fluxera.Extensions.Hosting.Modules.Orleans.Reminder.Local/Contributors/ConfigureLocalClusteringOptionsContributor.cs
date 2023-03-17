using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminder.Local.Contributors;

internal sealed class ConfigureLocalReminderOptionsContributor : ConfigureOptionsContributorBase<LocalReminderOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Reminder:Local";
}
