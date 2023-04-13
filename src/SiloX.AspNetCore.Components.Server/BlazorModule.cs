using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.AspNetCore;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SiloX.AspNetCore.Components.Server.Contributors;

namespace SiloX.AspNetCore.Components.Server;

/// <summary>
///     A module that enables Blazor server features for ASP.NET Core.
/// </summary>
[PublicAPI]
[DependsOn(typeof(AspNetCoreModule))]
public class BlazorModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureBlazorOptionsContributor>();
        context.Services.AddEndpointRouteContributor<EndpointRouteContributor>();
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        // var options = context.Services.GetOptions<ClusteringOptions>();
        // context.Log("AddOrleansClustering", services => services.AddOrleansClustering(options));
        context.Log("AddRazorPages", services => services.AddRazorPages());
        context.Log("AddServerSideBlazor", services => services.AddServerSideBlazor());
    }
}