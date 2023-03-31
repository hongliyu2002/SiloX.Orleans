using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using SiloX.Orleans;
using SiloX.Orleans.Clustering.InMemory;
using SiloX.Orleans.Clustering.Redis;
using SiloX.Orleans.EventSourcing.EventStore;
using SiloX.Orleans.EventSourcing.InMemory;
using SiloX.Orleans.Persistence.EventStore;
using SiloX.Orleans.Persistence.InMemory;
using SiloX.Orleans.Persistence.Redis;
using SiloX.Orleans.Reminders.InMemory;
using SiloX.Orleans.Reminders.Redis;
using SiloX.Orleans.Streaming.EventStore;
using SiloX.Orleans.Streaming.InMemory;
using Vending.Domain;
using Vending.Hosting.Contributors;
using Vending.Projection;

namespace Vending.Hosting;

[PublicAPI]
[DependsOn<ServerModule>]
[DependsOn<InMemoryClusteringModule>]
[DependsOn<InMemoryRemindersModule>]
[DependsOn<InMemoryEventSourcingModule>]
[DependsOn<InMemoryPersistenceModule>]
[DependsOn<InMemoryStreamingModule>]
[DependsOn<RedisClusteringModule>]
[DependsOn<RedisRemindersModule>]
[DependsOn<RedisPersistenceModule>]
[DependsOn<EventStoreEventSourcingModule>]
[DependsOn<EventStorePersistenceModule>]
[DependsOn<EventStoreStreamingModule>]
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
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var options = context.Services.GetOptions<HostingOptions>();
        context.Log("AddVendingHosting", services => services.AddVendingHosting(options));
    }
}
