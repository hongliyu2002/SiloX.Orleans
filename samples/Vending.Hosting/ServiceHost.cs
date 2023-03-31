﻿using System.Reflection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.Serilog;
using Fluxera.Extensions.Hosting.Plugins;
using Serilog;
using Serilog.Extensions.Logging;

namespace Vending.Hosting;

/// <summary>
///     Hosts the Vending Service.
/// </summary>
public sealed class ServiceHost : WebApplicationHost<HostingModule>
{
    /// <inheritdoc />
    protected override void ConfigureApplicationPlugins(IPluginConfigurationContext context)
    {
        // context.AddPlugin<SerilogModule>();
        // context.AddPlugin<HealthChecksEndpointsModule>();
    }

    /// <inheritdoc />
    protected override void ConfigureHostBuilder(IHostBuilder builder)
    {
        // Add user secrets configuration source.
        builder.ConfigureAppConfiguration(configurationBuilder =>
                                          {
                                              configurationBuilder.AddUserSecrets(Assembly.GetExecutingAssembly());
                                          });
        // Add Serilog logging
        builder.AddSerilogLogging((hostBuilderContext, loggerConfiguration) =>
                                  {
                                      loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration);
                                  });
    }

    /// <inheritdoc />
    protected override ILoggerFactory CreateBootstrapperLoggerFactory(IConfiguration configuration)
    {
        var bootstrapLogger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateBootstrapLogger();
        ILoggerFactory loggerFactory = new SerilogLoggerFactory(bootstrapLogger);
        return loggerFactory;
    }
}