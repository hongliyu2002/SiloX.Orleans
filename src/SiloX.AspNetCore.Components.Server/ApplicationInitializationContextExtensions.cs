﻿using Fluxera.Extensions.Hosting;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;

namespace SiloX.AspNetCore.Components.Server;

/// <summary>
///     Extension methods for the <see cref="IApplicationInitializationContext" /> type.
/// </summary>
[PublicAPI]
public static class ApplicationInitializationContextExtensions
{
    /// <summary>
    ///     Captures synchronous and asynchronous <see cref="Exception" /> instances from the pipeline and generates HTML error responses.
    /// </summary>
    public static IApplicationInitializationContext UseDeveloperExceptionPage(this IApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        context.Log("UseDeveloperExceptionPage", _ => app.UseDeveloperExceptionPage());
        return context;
    }

    /// <summary>
    ///     Adds a middleware to the pipeline that will catch exceptions, log them, reset the request path, and re-execute the request.
    ///     The request will not be re-executed if the response has already started.
    /// </summary>
    public static IApplicationInitializationContext UseExceptionHandler(this IApplicationInitializationContext context, string errorHandlingPath)
    {
        var app = context.GetApplicationBuilder();
        context.Log("UseExceptionHandler", _ => app.UseExceptionHandler(errorHandlingPath));
        return context;
    }

    /// <summary>
    ///     Adds a StatusCodePages middleware to the pipeline. Specifies that the response body should be generated by
    ///     re-executing the request pipeline using an alternate path. This path may contain a '{0}' placeholder of the status code.
    /// </summary>
    public static IApplicationInitializationContext UseStatusCodePagesWithReExecute(this IApplicationInitializationContext context, string pathFormat)
    {
        var app = context.GetApplicationBuilder();
        context.Log("UseStatusCodePagesWithReExecute", _ => app.UseStatusCodePagesWithReExecute(pathFormat));
        return context;
    }

    /// <summary>
    ///     Enables static file serving for the current request path
    /// </summary>
    public static IApplicationInitializationContext UseStaticFiles(this IApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        context.Log("UseStaticFiles", _ => app.UseStaticFiles());
        return context;
    }
}