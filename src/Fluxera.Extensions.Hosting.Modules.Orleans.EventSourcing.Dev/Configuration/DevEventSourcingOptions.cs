using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Orleans.Providers;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.EventSourcing.Dev;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class DevEventSourcingOptions
{
    /// <summary>
    ///     The log Consistency provider options.
    /// </summary>
    public DevEventSourcingLogConsistencyOptions[] LogConsistencies { get; set; } = Array.Empty<DevEventSourcingLogConsistencyOptions>();
}

/// <summary>
/// </summary>
[PublicAPI]
public sealed class DevEventSourcingLogConsistencyOptions
{
    /// <summary>
    ///     The name of the log Consistency provider.
    /// </summary>
    public string Name { get; set; } = ProviderConstants.DEFAULT_LOG_CONSISTENCY_PROVIDER_NAME;

    /// <summary>
    ///     The type of the log Consistency provider.
    /// </summary>
    [Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]
    public DevLogProvider LogProvider { get; set; } = DevLogProvider.LogBased;

    /// <summary>
    ///     The primary cluster of the custom storage based log Consistency provider.
    /// </summary>
    public string? PrimaryCluster { get; set; }
}
