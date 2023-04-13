using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.AspNetCore.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using SiloX.Orleans;
using SiloX.Orleans.Clustering.Redis;
using SiloX.Orleans.EventSourcing.EventStore;
using SiloX.Orleans.Persistence.EventStore;
using SiloX.Orleans.Persistence.Redis;
using SiloX.Orleans.Reminders.Redis;
using SiloX.Orleans.Streaming.EventStore;
using SiloX.Orleans.Transactions;
using Vending.Domain;
using Vending.Domain.Abstractions;
using Vending.Hosting.Contributors;
using Vending.Projection;
using Vending.Projection.Abstractions.Machines;
using Vending.Projection.Abstractions.Purchases;
using Vending.Projection.Abstractions.Snacks;

namespace Vending.Hosting;

[PublicAPI]
[DependsOn<HealthChecksEndpointsModule>]
[DependsOn<ServerModule>]
[DependsOn<RedisClusteringModule>]
[DependsOn<RedisRemindersModule>]
[DependsOn<RedisPersistenceModule>]
[DependsOn<EventStoreEventSourcingModule>]
[DependsOn<EventStorePersistenceModule>]
[DependsOn<EventStoreStreamingModule>]
[DependsOn<TransactionsModule>]
[DependsOn<DomainModule>]
[DependsOn<ProjectionModule>]
public sealed class HostingModule : ConfigureApplicationModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureHostingOptionsContributor>();
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        var options = context.Services.GetOptions<HostingOptions>();
        context.Log("AddVendingHosting", services => services.AddVendingHosting(options));
    }

    /// <inheritdoc />
    public override void Configure(IApplicationInitializationContext context)
    {
        // Register to application started event
        var appLifetime = context.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
        if (appLifetime != null)
        {
            var grainFactory = context.ServiceProvider.GetRequiredService<IGrainFactory>();
            appLifetime.ApplicationStarted.Register(() =>
                                                    {
                                                        // Add reminder for snack infos
                                                        var snackSynchronizerGrain = grainFactory.GetGrain<ISnackSynchronizerGrain>(string.Empty);
                                                        snackSynchronizerGrain.StartReminder(Constants.SnackInfosSyncDifferencesReminderName, TimeSpan.FromMinutes(1), TimeSpan.FromHours(1));
                                                        snackSynchronizerGrain.StartReminder(Constants.SnackInfosSyncAllReminderName, TimeSpan.FromHours(2), TimeSpan.FromHours(24));
                                                        // Add reminder for machine infos
                                                        var machineSynchronizerGrain = grainFactory.GetGrain<IMachineSynchronizerGrain>(string.Empty);
                                                        machineSynchronizerGrain.StartReminder(Constants.MachineInfosSyncDifferencesReminderName, TimeSpan.FromMinutes(1), TimeSpan.FromHours(1));
                                                        machineSynchronizerGrain.StartReminder(Constants.MachineInfosSyncAllReminderName, TimeSpan.FromHours(3), TimeSpan.FromHours(24));
                                                        // Add reminder for purchase infos
                                                        var purchaseSynchronizerGrain = grainFactory.GetGrain<IPurchaseSynchronizerGrain>(string.Empty);
                                                        purchaseSynchronizerGrain.StartReminder(Constants.PurchaseInfosSyncDifferencesReminderName, TimeSpan.FromMinutes(1), TimeSpan.FromHours(1));
                                                        purchaseSynchronizerGrain.StartReminder(Constants.PurchaseInfosSyncAllReminderName, TimeSpan.FromHours(12), TimeSpan.FromHours(48));
                                                    });
        }
    }
}