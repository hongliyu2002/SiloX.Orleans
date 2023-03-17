using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminders.Contributors;

internal sealed class ConfigureRemindersOptionsContributor : ConfigureOptionsContributorBase<RemindersOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Reminders";
}
