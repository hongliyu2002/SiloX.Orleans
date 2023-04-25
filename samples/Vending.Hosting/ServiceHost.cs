using System.Reflection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.AspNetCore.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.Serilog;
using Fluxera.Extensions.Hosting.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        context.AddPlugin<SerilogModule>();
        context.AddPlugin<HealthChecksEndpointsModule>();
    }

    /// <inheritdoc />
    protected override void ConfigureHostBuilder(IHostBuilder builder)
    {
        // Add user secrets configuration source.
        builder.ConfigureAppConfiguration(configuration => configuration.AddUserSecrets(Assembly.GetExecutingAssembly()));
        // Add Serilog logging
        builder.AddSerilogLogging();
    }

    /// <inheritdoc />
    protected override ILoggerFactory CreateBootstrapperLoggerFactory(IConfiguration configuration)
    {
        var logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).Enrich.FromLogContext().CreateBootstrapLogger();
        var loggerFactory = new SerilogLoggerFactory(logger);
        return loggerFactory;
    }
}