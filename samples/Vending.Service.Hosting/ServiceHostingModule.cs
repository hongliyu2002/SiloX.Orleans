using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
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
public sealed class ServiceHostingModule : ConfigureApplicationModule
{
}
