using System.Reflection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.AspNetCore.HealthChecks;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using Fluxera.Extensions.Hosting.Modules.Serilog;
using Fluxera.Extensions.Hosting.Plugins;
using OpenTelemetry.Logs;
using Serilog;
using Serilog.Extensions.Logging;

namespace Vending.Apps.Blazor;

/// <summary>
///     Hosts the Vending Client.
/// </summary>
public class AppHost : WebApplicationHost<AppModule>
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
        builder.ConfigureAppConfiguration(configuration =>
                                          {
                                              configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
                                          });
        // Add OpenTelemetry logging.
        builder.AddOpenTelemetryLogging(options =>
                                        {
                                            options.AddConsoleExporter();
                                        });
        // Add Serilog logging
        builder.AddSerilogLogging((context, configuration) =>
                                  {
                                      configuration.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext();
                                  });
    }

    /// <inheritdoc />
    protected override ILoggerFactory CreateBootstrapperLoggerFactory(IConfiguration configuration)
    {
        var logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).Enrich.FromLogContext().CreateBootstrapLogger();
        var loggerFactory = new SerilogLoggerFactory(logger);
        return loggerFactory;
    }
}