using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SiloX.AspNetCore.Components.DependencyInjection;
using SiloX.AspNetCore.Components.Web.Cookies;
using SiloX.AspNetCore.Components.Web.Utilities;
using SiloX.UI;

namespace SiloX.AspNetCore.Components.Web;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<UIModule>]
[DependsOn<ComponentsModule>]
public class ComponentsWebModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        context.Log("ReplaceWithServiceProviderComponentActivator", services => services.Replace(ServiceDescriptor.Transient<IComponentActivator, ServiceProviderComponentActivator>()));
        context.Log("AddCookieService", services => services.AddTransient<ICookieService, CookieService>());
        context.Log("AddUtilsService", services => services.AddTransient<IUtilsService, UtilsService>());
        context.Log("AddDefaultServerUrlProvider", services => services.AddSingleton<IServerUrlProvider, DefaultServerUrlProvider>());
    }
}