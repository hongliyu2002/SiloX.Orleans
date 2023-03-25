using Fluxera.Extensions.DataManagement;
using JetBrains.Annotations;
using Orleans.Providers;

namespace SiloX.Orleans.EventSourcing.EventStore;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class EventStoreEventSourcingOptions
{
    /// <summary>
    ///     The log Consistency provider options.
    /// </summary>
    public EventStoreEventSourcingLogConsistencyOptions[] LogConsistencyOptions { get; set; } = Array.Empty<EventStoreEventSourcingLogConsistencyOptions>();

    /// <summary>
    ///     Gets the connection strings.
    /// </summary>
    [Redact]
    public ConnectionStrings ConnectionStrings { get; internal set; } = new();
}

/// <summary>
/// </summary>
[PublicAPI]
public sealed class EventStoreEventSourcingLogConsistencyOptions
{
    /// <summary>
    ///     The name of the provider (also used as connection string name).
    /// </summary>
    public string ProviderName { get; set; } = ProviderConstants.DEFAULT_LOG_CONSISTENCY_PROVIDER_NAME;

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
    ///     The stage of silo lifecycle where storage should be initialized.  Storage must be initialized prior to use.
    /// </summary>
    public int InitStage { get; set; } = ServiceLifecycleStage.ApplicationServices;
}
