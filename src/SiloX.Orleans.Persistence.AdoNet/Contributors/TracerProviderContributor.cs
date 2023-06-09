﻿using Fluxera.Extensions.Hosting;
using Fluxera.Extensions.Hosting.Modules.OpenTelemetry;
using OpenTelemetry.Trace;

namespace SiloX.Orleans.Persistence.AdoNet.Contributors;

internal sealed class TracerProviderContributor : ITracerProviderContributor
{
    /// <inheritdoc />
    public void Configure(TracerProviderBuilder builder, IServiceConfigurationContext context)
    {
        // var persistenceOptions = context.Services.GetOptions<AdoNetPersistenceOptions>();
        // foreach (var storage in persistenceOptions.Storages)
        // {
        //     switch (storage.DbProvider)
        //     {
        //         case AdoNetDbProvider.SQLServer:
        //             builder.AddSqlClientInstrumentation(storage.ProviderName, instrumentationOptions => instrumentationOptions.RecordException = true);
        //             break;
        //         case AdoNetDbProvider.PostgreSQL:
        //             break;
        //         case AdoNetDbProvider.MySQL:
        //             builder.AddMySqlDataInstrumentation(instrumentationOptions => instrumentationOptions.RecordException = true);
        //             break;
        //         case AdoNetDbProvider.Oracle:
        //             break;
        //     }
        // }
    }
}
