using Fluxera.Extensions.DependencyInjection;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using Fluxera.Extensions.Hosting.Modules.Orleans.Streaming.InMemory.Contributors;
using JetBrains.Annotations;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Streaming.InMemory;

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
        var streamingOptions = context.Services.GetObject<InMemoryStreamingOptions>();
        context.Log("AddOrleansInMemoryStreaming", services => services.AddOrleansInMemoryStreaming(streamingOptions));
    }
}
