using Fluxera.Extensions.Hosting.Modules.Configuration;

namespace SiloX.Extensions.Hosting.Modules.Orleans.Reminders.Contributors;

internal sealed class ConfigureRemindersOptionsContributor : ConfigureOptionsContributorBase<RemindersOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Reminders";
}
