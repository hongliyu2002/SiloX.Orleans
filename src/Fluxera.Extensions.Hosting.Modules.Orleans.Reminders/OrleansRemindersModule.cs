using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.Orleans.Reminders.Contributors;
using JetBrains.Annotations;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminders;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<ConfigurationModule>]
public class OrleansRemindersModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureRemindersOptionsContributor>();
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        var remindersOptions = context.Services.GetOptions<RemindersOptions>();
        context.Log("AddOrleansReminders", services => services.AddOrleansReminders(remindersOptions));
    }
}
