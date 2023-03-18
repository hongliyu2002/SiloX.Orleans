﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.Local;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansLocalPersistence(this IServiceCollection services, LocalPersistenceOptions options)
    {
        return services.AddOrleans(siloBuilder =>
                                   {
                                       foreach (var storage in options.Storages)
                                       {
                                           siloBuilder.AddMemoryGrainStorage(storage.Name,
                                                                             storageOptions =>
                                                                             {
                                                                                 storageOptions.NumStorageGrains = storage.NumStorageGrains;
                                                                                 storageOptions.InitStage = storage.InitStage;
                                                                             });
                                       }
                                   });
    }
}