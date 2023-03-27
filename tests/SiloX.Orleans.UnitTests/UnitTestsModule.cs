using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using SiloX.Orleans.Clustering.InMemory;
using SiloX.Orleans.Clustering.Redis;
using SiloX.Orleans.EventSourcing.EventStore;
using SiloX.Orleans.EventSourcing.InMemory;
using SiloX.Orleans.Persistence.EventStore;
using SiloX.Orleans.Persistence.InMemory;
using SiloX.Orleans.Reminders.InMemory;
using SiloX.Orleans.Reminders.Redis;
using SiloX.Orleans.Streaming.EventStore;
using SiloX.Orleans.Streaming.InMemory;

namespace SiloX.Orleans.UnitTests;

[PublicAPI]
[DependsOn<OrleansServerModule>]
[DependsOn<OrleansInMemoryClusteringModule>]
[DependsOn<OrleansInMemoryRemindersModule>]
[DependsOn<OrleansInMemoryEventSourcingModule>]
[DependsOn<OrleansInMemoryPersistenceModule>]
[DependsOn<OrleansInMemoryStreamingModule>]
[DependsOn<ConfigurationModule>]
public class UnitTestsModule : ConfigureServicesModule
{
}
