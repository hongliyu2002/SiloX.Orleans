using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.AspNetCore;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace SiloX.AspNetCore.Components.Server.Contributors;

[UsedImplicitly]
internal sealed class EndpointRouteContributor : IEndpointRouteContributor
{
    public int Position => 1000;
    
    /// <inheritdoc />
    public void MapRoute(IEndpointRouteBuilder routeBuilder, IApplicationInitializationContext context)
    {
        context.Log("MapBlazorHub", _ => routeBuilder.MapBlazorHub());
        context.Log("MapFallbackToPage", _ => routeBuilder.MapFallbackToPage("/_Host"));
    }
}