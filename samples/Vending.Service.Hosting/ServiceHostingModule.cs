using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using JetBrains.Annotations;
using SiloX.Orleans;
using SiloX.Orleans.Clustering.InMemory;
using SiloX.Orleans.EventSourcing.InMemory;
using SiloX.Orleans.Persistence.InMemory;
using SiloX.Orleans.Reminders.InMemory;
using SiloX.Orleans.Streaming.InMemory;

namespace Vending.Hosting;

[PublicAPI]
[DependsOn<OrleansServerModule>]
[DependsOn<OrleansInMemoryClusteringModule>]
[DependsOn<OrleansInMemoryRemindersModule>]
[DependsOn<OrleansInMemoryEventSourcingModule>]
[DependsOn<OrleansInMemoryPersistenceModule>]
[DependsOn<OrleansInMemoryStreamingModule>]
[DependsOn<DomainModule>]
public sealed class ServiceHostingModule : ConfigureApplicationModule
{
}
