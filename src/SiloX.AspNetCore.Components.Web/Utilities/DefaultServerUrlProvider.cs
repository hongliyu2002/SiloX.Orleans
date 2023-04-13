using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Web.Utilities;

/// <summary>
///     Default implementation of <see cref="IServerUrlProvider" /> that always returns "/".
/// </summary>
[PublicAPI]
public class DefaultServerUrlProvider : IServerUrlProvider
{
    /// <inheritdoc />
    public Task<string> GetBaseUrlAsync(string? remoteServiceName = null)
    {
        return Task.FromResult("/");
    }
}