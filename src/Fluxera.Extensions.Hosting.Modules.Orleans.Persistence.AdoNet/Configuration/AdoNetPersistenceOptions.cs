using System.Text.Json.Serialization;
using Fluxera.Extensions.DataManagement;
using JetBrains.Annotations;
using Orleans.Providers;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.AdoNet;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class AdoNetPersistenceOptions
{
    /// <summary>
    ///     The storage descriptions.
    /// </summary>
    public StorageSettings[] Storages { get; set; } = Array.Empty<StorageSettings>();

    /// <summary>
    ///     Gets the connection strings.
    /// </summary>
    [Redact]
    public ConnectionStrings ConnectionStrings { get; internal set; } = new();

    /// <summary>
    /// </summary>
    public class StorageSettings
    {
        /// <summary>
        ///     The name of the connection string.
        /// </summary>
        public string ConnectionStringName { get; set; } = ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME;

        /// <summary>
        ///     The name and database provider type of the connection string.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AdoNetDbProvider DbProvider { get; set; } = AdoNetDbProvider.SQLServer;

        /// <summary>
        ///     The stage of silo lifecycle where storage should be initialized.  Storage must be initialized prior to use.
        /// </summary>
        public int InitStage { get; set; } = 10000;
    }
}
