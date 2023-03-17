using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.Orleans.Reminder.Contributors;
using JetBrains.Annotations;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminder;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<ConfigurationModule>]
public class OrleansReminderModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureReminderOptionsContributor>();
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        var reminderOptions = context.Services.GetOptions<ReminderOptions>();
        context.Log("AddOrleansReminder", services => services.AddOrleansReminder(reminderOptions));
    }
}
