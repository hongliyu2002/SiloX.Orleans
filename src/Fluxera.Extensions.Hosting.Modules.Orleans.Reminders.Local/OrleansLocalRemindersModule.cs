using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.Orleans.Reminders.Local.Contributors;
using JetBrains.Annotations;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminders.Local;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<ConfigurationModule>]
public class OrleansLocalRemindersModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureLocalRemindersOptionsContributor>();
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        var remindersOptions = context.Services.GetObject<LocalRemindersOptions>();
        context.Log("AddOrleansLocalReminders", services => services.AddOrleansLocalReminders(remindersOptions));
    }
}
