﻿using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules;
using Fluxera.Extensions.Hosting.Modules.Configuration;
using JetBrains.Annotations;
using SiloX.Orleans.EventSourcing.Contributors;

namespace SiloX.Orleans.EventSourcing;

/// <summary>
/// </summary>
[PublicAPI]
[DependsOn<ConfigurationModule>]
public class EventSourcingModule : ConfigureServicesModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(IServiceConfigurationContext context)
    {
        context.Services.AddConfigureOptionsContributor<ConfigureEventSourcingOptionsContributor>();
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceConfigurationContext context)
    {
        var options = context.Services.GetOptions<EventSourcingOptions>();
        context.Log("AddOrleansEventSourcing", services => services.AddOrleansEventSourcing(options));
    }
}
