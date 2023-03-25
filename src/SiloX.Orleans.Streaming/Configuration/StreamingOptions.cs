using JetBrains.Annotations;
using Orleans.Providers;

namespace SiloX.Orleans.Streaming;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class StreamingOptions
{
    /// <summary>
    ///     Is this configuration intended for client-side use?
    /// </summary>
    public bool UsedByClient { get; set; }

    /// <summary>
    ///     The broadcast channel options.
    /// </summary>
    public BroadcastOptions[] Broadcasts { get; set; } = Array.Empty<BroadcastOptions>();
}

/// <summary>
/// </summary>
[PublicAPI]
public sealed class BroadcastOptions
{
    /// <summary>
    ///     The name of the provider (also used as connection string name).
    /// </summary>
    public string ProviderName { get; set; } = ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME;

    /// <summary>
    ///     If set to true, the provider will not await calls to subscriber.
    /// </summary>
    public bool FireAndForgetDelivery { get; set; } = true;
}
