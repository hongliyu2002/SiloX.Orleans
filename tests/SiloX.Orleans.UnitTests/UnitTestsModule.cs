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
[DependsOn<OrleansClientModule>]
[DependsOn<OrleansRedisClusteringModule>]
[DependsOn<OrleansRedisRemindersModule>]
[DependsOn<OrleansEventStoreEventSourcingModule>]
[DependsOn<OrleansEventStorePersistenceModule>]
[DependsOn<OrleansEventStoreStreamingModule>]
[DependsOn<ConfigurationModule>]
public class UnitTestsModule : ConfigureApplicationModule
{
}
