using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using SiloX.Orleans.Reminders.InMemory.Contributors;

namespace SiloX.Orleans.Reminders.InMemory;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<OrleansRemindersModule>]
[DependsOn<ConfigurationModule>]
public class OrleansInMemoryRemindersModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureInMemoryRemindersOptionsContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var inMemoryOptions = context.Services.GetObject<InMemoryRemindersOptions>();
        context.Log("AddOrleansInMemoryReminders", services => services.AddOrleansInMemoryReminders(inMemoryOptions));
    }
}
