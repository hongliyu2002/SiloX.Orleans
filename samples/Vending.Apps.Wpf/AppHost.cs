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
        builder.AddSerilogLogging((context, configuration) =>
                                  {
                                      configuration.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext();
                                  });
        // Register the main window factory to use.
        builder.UseMainWindow(sp => new MainWindow());
    }
}
