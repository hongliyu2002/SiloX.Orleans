using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace SiloX.Orleans.Reminders.Contributors;

internal sealed class ConfigureRemindersOptionsContributor : ConfigureOptionsContributorBase<RemindersOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Reminders";
}
