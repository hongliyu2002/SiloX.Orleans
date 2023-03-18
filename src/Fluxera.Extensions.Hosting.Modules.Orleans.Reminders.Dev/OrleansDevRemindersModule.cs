using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.Orleans.Reminders.Dev.Contributors;
using JetBrains.Annotations;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminders.Dev;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<OrleansRemindersModule>]
[DependsOn<ConfigurationModule>]
public class OrleansDevRemindersModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureDevRemindersOptionsContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var remindersOptions = context.Services.GetObject<DevRemindersOptions>();
        context.Log("AddOrleansDevReminders", services => services.AddOrleansDevReminders(remindersOptions));
    }
}
