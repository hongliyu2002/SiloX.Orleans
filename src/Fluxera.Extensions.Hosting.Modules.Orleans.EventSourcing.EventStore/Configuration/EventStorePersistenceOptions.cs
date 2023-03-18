﻿using Fluxera.Extensions.DataManagement;
using JetBrains.Annotations;
using Orleans.Providers;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.EventSourcing.EventStore;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class EventStoreEventSourcingOptions
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
        ///     The stage of silo lifecycle where storage should be initialized.  Storage must be initialized prior to use.
        /// </summary>
        public int InitStage { get; set; } = 10000;
    }
}
