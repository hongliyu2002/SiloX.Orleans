using System.Reflection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using SiloX.Orleans;
using SiloX.Orleans.Clustering.Redis;
using SiloX.Orleans.Streaming.EventStore;
using SiloX.Orleans.Transactions;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Vending.App.Wpf.Contributors;
using Vending.App.Wpf.Services;

namespace Vending.App.Wpf;

[PublicAPI]
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
        context.Log("AddVendingWpf", services => services.AddVendingWpf(options));
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.UseMicrosoftDependencyResolver();
        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();
        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());
    }

    /// <inheritdoc />
    public override void Configure(IApplicationInitializationContext context)
    {
        var services = context.ServiceProvider;
        // Use Microsoft Dependency Resolver for ReactiveUI
        services.UseMicrosoftDependencyResolver();
        // Register application lifetime events
        var appLifetime = services.GetRequiredService<IHostApplicationLifetime>();
        appLifetime.ApplicationStarted.Register(() =>
                                                {
                                                    var clusterClientReady = services.GetRequiredService<IClusterClientReady>();
                                                    var clusterClient = services.GetRequiredService<IClusterClient>();
                                                    clusterClientReady.SetClusterClient(clusterClient);
                                                });
    }
}
