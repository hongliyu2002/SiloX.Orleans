using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.Serilog;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Vending.App.Wpf;

/// <summary>
///     Hosts the Vending Client.
/// </summary>
public sealed class AppHost : WpfApplicationHost<AppModule>
{
    /// <inheritdoc />
    protected override void ConfigureHostBuilder(IHostBuilder builder)
    {
        base.ConfigureHostBuilder(builder);
        // Add Serilog logging
        builder.AddSerilogLogging((hostBuilderContext, loggerConfiguration) =>
                                  {
                                      loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration);
                                  });
        // Register the main window factory to use.
        builder.UseMainWindow(sp => new MainWindow());
    }
}
