using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using SiloX.Orleans.Clustering.Redis;
using SiloX.Orleans.EventSourcing.EventStore;
using SiloX.Orleans.Persistence.EventStore;
using SiloX.Orleans.Reminders.Redis;
using SiloX.Orleans.Streaming.EventStore;

namespace SiloX.Orleans.UnitTests;

[PublicAPI]
[DependsOn<ClientModule>]
[DependsOn<RedisClusteringModule>]
[DependsOn<RedisRemindersModule>]
[DependsOn<EventStoreEventSourcingModule>]
[DependsOn<EventStorePersistenceModule>]
[DependsOn<EventStoreStreamingModule>]
[DependsOn<ConfigurationModule>]
public class UnitTestsModule : ConfigureApplicationModule
{
}
