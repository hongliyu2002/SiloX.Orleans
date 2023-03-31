using System.Reflection;
using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.Serilog;
using Fluxera.Extensions.Hosting.Plugins;
using Serilog;
using Serilog.Extensions.Logging;

namespace Vending.Hosting;

public class ServiceHost : WebApplicationHost<ServiceHostingModule>
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
        builder.ConfigureAppConfiguration(configurationBuilder => { configurationBuilder.AddUserSecrets(Assembly.GetExecutingAssembly()); });
        // Add Serilog logging
        builder.AddSerilogLogging((hostBuilderContext, loggerConfiguration) => { loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration).Enrich.FromLogContext(); });
    }

    /// <inheritdoc />
    protected override ILoggerFactory CreateBootstrapperLoggerFactory(IConfiguration configuration)
    {
        var bootstrapLogger = new LoggerConfiguration().ReadFrom.Configuration(configuration).Enrich.FromLogContext().CreateBootstrapLogger();
        ILoggerFactory loggerFactory = new SerilogLoggerFactory(bootstrapLogger);
        return loggerFactory;
    }
}
