using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using OpenTelemetry.Trace;

namespace SiloX.Extensions.Hosting.Modules.Orleans.Reminders.AdoNet.Contributors;

internal sealed class TracerProviderContributor : ITracerProviderContributor
{
    /// <inheritdoc />
    public void Configure(TracerProviderBuilder builder, IServiceConfigurationContext context)
    {
        // var options = context.Services.GetOptions<AdoNetRemindersOptions>();
        // switch (options.DbProvider)
        // {
        //     case AdoNetDbProvider.SQLServer:
        //         builder.AddSqlClientInstrumentation();
        //         break;
        //     case AdoNetDbProvider.PostgreSQL:
        //         break;
        //     case AdoNetDbProvider.MySQL:
        //         builder.AddMySqlDataInstrumentation();
        //         break;
        //     case AdoNetDbProvider.Oracle:
        //         break;
        // }
    }
}
