using Fluxera.Extensions.DataManagement;
using JetBrains.Annotations;
using Orleans.Providers;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.EventStore;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class EventStorePersistenceOptions
{
    /// <summary>
    ///     The storage options.
    /// </summary>
    public EventStorePersistenceStorageOptions[] Storages { get; set; } = Array.Empty<EventStorePersistenceStorageOptions>();

    /// <summary>
    ///     Gets the connection strings.
    /// </summary>
    [Redact]
    public ConnectionStrings ConnectionStrings { get; internal set; } = new();

}

/// <summary>
/// </summary>
[PublicAPI]
public sealed class EventStorePersistenceStorageOptions
{
    /// <summary>
    ///     The name of the connection string.
    /// </summary>
    public string ConnectionStringName { get; set; } = ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME;

    /// <summary>
    ///     The user name of credentials that have permissions to append events.
    /// </summary>
    [Redact]
    public string? Username { get; set; }

    /// <summary>
    ///     The password of credentials that have permissions to append events.
    /// </summary>
    [Redact]
    public string? Password { get; set; }

    /// <summary>
    ///     Whether or not to delete state during a clear operation.
    /// </summary>
    public bool DeleteStateOnClear { get; set; }

    /// <summary>
    ///     The stage of silo lifecycle where storage should be initialized.  Storage must be initialized prior to use.
    /// </summary>
    public int InitStage { get; set; } = ServiceLifecycleStage.ApplicationServices;
}
