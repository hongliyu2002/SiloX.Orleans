using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using SiloX.Orleans.Streaming.InMemory.Contributors;

namespace SiloX.Orleans.Streaming.InMemory;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<OrleansStreamingModule>]
[DependsOn<ConfigurationModule>]
public class OrleansInMemoryStreamingModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureInMemoryStreamingOptionsContributor>();
    }

    /// <inheritdoc />
    public override void PostConfigureServices(IServiceConfigurationContext context)
    {
        var options = context.Services.GetObject<StreamingOptions>();
        var inMemoryOptions = context.Services.GetObject<InMemoryStreamingOptions>();
        context.Log("AddOrleansInMemoryStreaming", services => services.AddOrleansInMemoryStreaming(options, inMemoryOptions));
    }
}
