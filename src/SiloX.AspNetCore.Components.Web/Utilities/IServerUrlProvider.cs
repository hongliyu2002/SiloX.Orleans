using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Web.Utilities;

/// <summary>
///     Provides the base url of the current server
/// </summary>
[PublicAPI]
public interface IServerUrlProvider
{
    /// <summary>
    ///     Get the base url of the current server
    /// </summary>
    /// <param name="remoteServiceName">The name of the remote service to get the base url for.</param>
    /// <returns>The base url of the current server</returns>
    Task<string> GetBaseUrlAsync(string? remoteServiceName = null);
}