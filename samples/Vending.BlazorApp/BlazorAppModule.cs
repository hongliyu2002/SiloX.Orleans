using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.AspNetCore;
using Fluxera.Extensions.Hosting.Modules.AspNetCore.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using SiloX.AspNetCore.Components.Server;
using SiloX.Orleans;
using SiloX.Orleans.Clustering.Redis;
using SiloX.Orleans.Streaming.EventStore;
using SiloX.Orleans.Transactions;
using Vending.BlazorApp.Contributors;

namespace Vending.BlazorApp;

[PublicAPI]
[DependsOn<BlazorModule>]
[DependsOn<HealthChecksEndpointsModule>]
[DependsOn<ClientModule>]
[DependsOn<RedisClusteringModule>]
[DependsOn<EventStoreStreamingModule>]
[DependsOn<TransactionsModule>]
public sealed class BlazorAppModule : ConfigureApplicationModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureBlazorAppOptionsContributor>();
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        var options = context.Services.GetOptions<BlazorAppOptions>();
        context.Log("AddVendingBlazorApp", services => services.AddVendingBlazorApp(options));
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
            context.UseExceptionHandler("/Error");
            context.UseHsts();
        }
        // context.UseHttpsRedirection();
        context.UseStaticFiles();
        context.UseRouting();
        context.UseEndpoints();
    }
}