using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using OpenTelemetry.Trace;

namespace SiloX.Orleans.Clustering.AdoNet.Contributors;

internal sealed class TracerProviderContributor : ITracerProviderContributor
{
    /// <inheritdoc />
    public void Configure(TracerProviderBuilder builder, IServiceConfigurationContext context)
    {
        // var options = context.Services.GetOptions<AdoNetClusteringOptions>();
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
