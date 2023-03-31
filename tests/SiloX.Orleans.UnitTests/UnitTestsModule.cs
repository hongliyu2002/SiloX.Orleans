using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using JetBrains.Annotations;
using SiloX.Orleans.Clustering.Redis;
using SiloX.Orleans.Streaming.EventStore;
using SiloX.Orleans.Transactions;

namespace SiloX.Orleans.UnitTests;

[PublicAPI]
[DependsOn<ClientModule>]
[DependsOn<RedisClusteringModule>]
[DependsOn<EventStoreStreamingModule>]
[DependsOn<TransactionsModule>]
public sealed class UnitTestsModule : ConfigureApplicationModule
{
}
