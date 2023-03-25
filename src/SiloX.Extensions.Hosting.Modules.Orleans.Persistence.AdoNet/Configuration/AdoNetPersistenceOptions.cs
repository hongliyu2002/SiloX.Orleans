using System.Text.Json.Serialization;
using Fluxera.Extensions.DataManagement;
using JetBrains.Annotations;
using Orleans.Providers;

namespace SiloX.Extensions.Hosting.Modules.Orleans.Persistence.AdoNet;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class AdoNetPersistenceOptions
{
    /// <summary>
    ///     The storage options.
    /// </summary>
    public AdoNetPersistenceStorageOptions[] Storages { get; set; } = Array.Empty<AdoNetPersistenceStorageOptions>();

    /// <summary>
    ///     Gets the connection strings.
    /// </summary>
    [Redact]
    public ConnectionStrings ConnectionStrings { get; internal set; } = new();
}

/// <summary>
/// </summary>
[PublicAPI]
public sealed class AdoNetPersistenceStorageOptions
{
    /// <summary>
    ///     The name of the provider (also used as connection string name).
    /// </summary>
    public string ProviderName { get; set; } = ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME;

    /// <summary>
    ///     The name and database provider type of the connection string.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AdoNetDbProvider DbProvider { get; set; } = AdoNetDbProvider.SQLServer;

    /// <summary>
    ///     The stage of silo lifecycle where storage should be initialized.  Storage must be initialized prior to use.
    /// </summary>
    public int InitStage { get; set; } = ServiceLifecycleStage.ApplicationServices;
}
