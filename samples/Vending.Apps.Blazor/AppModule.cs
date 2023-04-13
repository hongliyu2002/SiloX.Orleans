using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.AspNetCore;
using Fluxera.Extensions.Hosting.Modules.AspNetCore.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using MudBlazor.Services;
using SiloX.AspNetCore.Components.Server;
using SiloX.Orleans;
using SiloX.Orleans.Clustering.Redis;
using SiloX.Orleans.Streaming.EventStore;
using SiloX.Orleans.Transactions;
using Vending.Apps.Blazor.Contributors;

namespace Vending.Apps.Blazor;

[PublicAPI]
[DependsOn<BlazorModule>]
[DependsOn<HealthChecksEndpointsModule>]
[DependsOn<ClientModule>]
[DependsOn<RedisClusteringModule>]
[DependsOn<EventStoreStreamingModule>]
[DependsOn<TransactionsModule>]
public sealed class AppModule : ConfigureApplicationModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureAppOptionsContributor>();
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        var options = context.Services.GetOptions<AppOptions>();
        context.Log("AddVendingApp", services => services.AddVendingApp(options));
        context.Log("AddMudServices", services => services.AddMudServices());
    }

    /// <inheritdoc />
    public override void Configure(IApplicationInitializationContext context)
    {
        if (context.Environment.IsDevelopment())
        {
            context.UseDeveloperExceptionPage();
        }
        else
        {
            context.UseExceptionHandler("/error");
            context.UseHsts();
        }
        // context.UseHttpsRedirection();
        context.UseStaticFiles();
        context.UseRouting();
        context.UseEndpoints();
    }
}