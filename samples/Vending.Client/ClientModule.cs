using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using SiloX.Orleans.Clustering.Redis;
using SiloX.Orleans.Streaming.EventStore;
using SiloX.Orleans.Transactions;
using Vending.Client.Contributors;

namespace Vending.Client;

[PublicAPI]
[DependsOn<ClientModule>]
[DependsOn<RedisClusteringModule>]
[DependsOn<EventStoreStreamingModule>]
[DependsOn<TransactionsModule>]
public sealed class ClientModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureClientOptionsContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var options = context.Services.GetOptions<ClientOptions>();
        context.Log("AddVendingClient", services => services.AddVendingClient(options));
    }
}
