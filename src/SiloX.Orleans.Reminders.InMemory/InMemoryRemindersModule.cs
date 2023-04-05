using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using SiloX.Orleans.Reminders.InMemory.Contributors;

namespace SiloX.Orleans.Reminders.InMemory;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<RemindersModule>]
[DependsOn<ConfigurationModule>]
public class InMemoryRemindersModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureInMemoryRemindersOptionsContributor>();
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        var inMemoryOptions = context.Services.GetOptions<InMemoryRemindersOptions>();
        context.Log("AddOrleansInMemoryReminders", services => services.AddOrleansInMemoryReminders(inMemoryOptions));
    }
}
