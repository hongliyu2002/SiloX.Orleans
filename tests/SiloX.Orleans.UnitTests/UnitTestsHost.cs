using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace SiloX.Orleans.UnitTests;

public class UnitTestsHost : TestApplicationHost<UnitTestsModule>
{
    /// <inheritdoc />
    protected override void ConfigureApplicationPlugins(IPluginConfigurationContext context)
    {
    }

    /// <inheritdoc />
    protected override ILoggerFactory CreateBootstrapperLoggerFactory(IConfiguration configuration)
    {
        var bootstrapLogger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateBootstrapLogger();
        ILoggerFactory loggerFactory = new SerilogLoggerFactory(bootstrapLogger);
        return loggerFactory;
    }
}
