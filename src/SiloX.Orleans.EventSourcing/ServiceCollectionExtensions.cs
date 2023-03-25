﻿using Microsoft.Extensions.DependencyInjection;

namespace SiloX.Orleans.EventSourcing;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOrleansEventSourcing(this IServiceCollection services, EventSourcingOptions options)
    {
        // return services.AddOrleans(siloBuilder =>
        //                            {
        //                            });
        return services;
    }
}
