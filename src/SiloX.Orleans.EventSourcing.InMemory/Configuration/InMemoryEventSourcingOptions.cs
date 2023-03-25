using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Orleans.Providers;

namespace SiloX.Orleans.EventSourcing.InMemory;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class InMemoryEventSourcingOptions
{
    /// <summary>
    ///     The log Consistency provider options.
    /// </summary>
    public InMemoryEventSourcingLogConsistencyOptions[] LogConsistencies { get; set; } = Array.Empty<InMemoryEventSourcingLogConsistencyOptions>();
}

/// <summary>
/// </summary>
[PublicAPI]
public sealed class InMemoryEventSourcingLogConsistencyOptions
{
    /// <summary>
    ///     The name of the provider (also used as connection string name).
    /// </summary>
    public string ProviderName { get; set; } = ProviderConstants.DEFAULT_LOG_CONSISTENCY_PROVIDER_NAME;

    /// <summary>
    ///     The type of the log Consistency provider.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public InMemoryLogProvider LogProvider { get; set; } = InMemoryLogProvider.LogBased;

    /// <summary>
    ///     The primary cluster of the custom storage based log Consistency provider.
    /// </summary>
    public string? PrimaryCluster { get; set; }
}
