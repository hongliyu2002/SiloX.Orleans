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
using Vending.Apps.Blazor.Services;

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
        context.Log("AddVendingBlazor", services => services.AddVendingBlazor(options));
        context.Log("AddMudServices", services => services.AddMudServices());
    }

    /// <inheritdoc />
    public override void Configure(IApplicationInitializationContext context)
    {
        // Register application lifetime events
        var services = context.ServiceProvider;
        var appLifetime = services.GetRequiredService<IHostApplicationLifetime>();
        appLifetime.ApplicationStarted.Register(() =>
                                                {
                                                    var clusterClientReady = services.GetRequiredService<IClusterClientReady>();
                                                    var clusterClient = services.GetRequiredService<IClusterClient>();
                                                    clusterClientReady.SetClusterClient(clusterClient);
                                                });

        // Configure application
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