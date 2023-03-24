using JetBrains.Annotations;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Streaming;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class StreamingOptions
{
    /// <summary>
    ///     Is this configuration intended for client-side use?
    /// </summary>
    public bool UsedByClient { get; set; }

}
