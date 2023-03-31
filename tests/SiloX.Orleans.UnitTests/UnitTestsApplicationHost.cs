// using Fluxera.Extensions.Hosting;
// using Fluxera.Extensions.Hosting.Modules.AspNetCore.HealthChecks;
// using Fluxera.Extensions.Hosting.Modules.Serilog;
// using Fluxera.Extensions.Hosting.Plugins;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
// using Serilog;
// using Serilog.Extensions.Logging;
//
// namespace SiloX.Orleans.UnitTests;
//
// public class UnitTestsApplicationHost : TestApplicationHost<UnitTestsModule>
// {
//     /// <inheritdoc />
//     protected override void ConfigureApplicationPlugins(IPluginConfigurationContext context)
//     {
//         context.AddPlugin<SerilogModule>();
//         context.AddPlugin<HealthChecksEndpointsModule>();
//     }
//
//     /// <inheritdoc />
//     [Obsolete("Obsolete")]
//     protected override void ConfigureHostBuilder(IWebHostBuilder builder)
//     {
//         // Add Serilog logging
//         builder.UseSerilog((hostBuilderContext, loggerConfiguration) =>
//                            {
//                                loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration).Enrich.FromLogContext().WriteTo.Console();
//                            });
//     }
//
//     /// <inheritdoc />
//     protected override ILoggerFactory CreateBootstrapperLoggerFactory(IConfiguration configuration)
//     {
//         var bootstrapLogger = new LoggerConfiguration().ReadFrom.Configuration(configuration).Enrich.FromLogContext().WriteTo.Console().CreateBootstrapLogger();
//         ILoggerFactory loggerFactory = new SerilogLoggerFactory(bootstrapLogger);
//         return loggerFactory;
//     }
// }
