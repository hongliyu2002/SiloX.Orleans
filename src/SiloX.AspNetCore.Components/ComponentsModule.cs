using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SiloX.AspNetCore.Components.ExceptionHandling;
using SiloX.AspNetCore.Components.Notifications;
using SiloX.AspNetCore.Components.Progression;

namespace SiloX.AspNetCore.Components;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<ConfigurationModule>]
public class ComponentsModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        context.Log("AddNullUserExceptionInformer", services => services.AddSingleton<IUserExceptionInformer, NullUserExceptionInformer>());
        context.Log("AddNNullUiNotificationService", services => services.AddTransient<IUiNotificationService, NullUiNotificationService>());
        context.Log("AddNullUiPageProgressService", services => services.AddSingleton<IUiPageProgressService, NullUiPageProgressService>());
    }
}