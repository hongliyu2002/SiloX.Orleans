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
    public LogConsistencyProviderDescription[] LogConsistencyProviders { get; set; } = Array.Empty<LogConsistencyProviderDescription>();

    /// <summary>
    /// </summary>
    public class LogConsistencyProviderDescription
    {
        /// <summary>
        ///     The name of the log Consistency provider.
        /// </summary>
        public string Name { get; set; } = ProviderConstants.DEFAULT_LOG_CONSISTENCY_PROVIDER_NAME;

        /// <summary>
        ///     The type of the log Consistency provider.
        /// </summary>
        [Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]
        public DevLogConsistencyProvider Provider { get; set; } = DevLogConsistencyProvider.LogStorageBased;

        /// <summary>
        ///     The name of the log Consistency provider.
        /// </summary>
        public string? PrimaryCluster { get; set; }
    }
}
