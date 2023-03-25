using Fluxera.Extensions.DataManagement;
using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SiloX.Orleans.Clustering.Redis.Contributors;

internal sealed class ConfigureRedisClusteringOptionsContributor : ConfigureOptionsContributorBase<RedisClusteringOptions>
{
    /// <inheritdoc />
    public override string SectionName => "Orleans:Clustering:Redis";

    /// <inheritdoc />
    protected override void AdditionalConfigure(IServiceConfigurationContext context, RedisClusteringOptions createdOptions)
    {
        createdOptions.ConnectionStrings = context.Services.GetObject<ConnectionStrings>();
        context.Log("Configure(RedisClusteringOptions)",
                    services =>
                    {
                        services.Configure<RedisClusteringOptions>(clustering =>
                                                                   {
                                                                       clustering.ConnectionStrings = createdOptions.ConnectionStrings;
                                                                   });
                    });
    }
}
