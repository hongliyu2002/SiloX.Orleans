using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using SiloX.Extensions.Hosting.Modules.Orleans.Reminders.Contributors;
using JetBrains.Annotations;

namespace SiloX.Extensions.Hosting.Modules.Orleans.Reminders;

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
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var remindersOptions = context.Services.GetOptions<RemindersOptions>();
        context.Log("AddOrleansReminders", services => services.AddOrleansReminders(remindersOptions));
    }
}
