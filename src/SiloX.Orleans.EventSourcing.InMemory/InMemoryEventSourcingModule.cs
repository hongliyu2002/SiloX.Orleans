﻿using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using SiloX.Orleans.EventSourcing.InMemory.Contributors;

namespace SiloX.Orleans.EventSourcing.InMemory;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<EventSourcingModule>]
[DependsOn<ConfigurationModule>]
public class InMemoryEventSourcingModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureInMemoryEventSourcingOptionsContributor>();
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        var inMemoryOptions = context.Services.GetOptions<InMemoryEventSourcingOptions>();
        context.Log("AddOrleansInMemoryEventSourcing", services => services.AddOrleansInMemoryEventSourcing(inMemoryOptions));
    }
}
