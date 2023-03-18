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
    ///     The storage descriptions.
    /// </summary>
    public LogConsistencySettings[] LogConsistencies { get; set; } = Array.Empty<LogConsistencySettings>();

    /// <summary>
    /// </summary>
    public class LogConsistencySettings
    {
        /// <summary>
        ///     The name of the log Consistency provider.
        /// </summary>
        public string Name { get; set; } = ProviderConstants.DEFAULT_LOG_CONSISTENCY_PROVIDER_NAME;

        /// <summary>
        ///     The type of the log Consistency provider.
        /// </summary>
        [Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]
        public DevLogConsistencyProvider LogProvider { get; set; } = DevLogConsistencyProvider.LogStorageBased;

        /// <summary>
        ///     The primary cluster of the custom storage based log Consistency provider.
        /// </summary>
        public string? PrimaryCluster { get; set; }
    }
}
