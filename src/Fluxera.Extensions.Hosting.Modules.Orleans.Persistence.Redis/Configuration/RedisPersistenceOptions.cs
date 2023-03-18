﻿using Fluxera.Extensions.DataManagement;
using JetBrains.Annotations;
using Orleans.Providers;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.Redis;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class RedisPersistenceOptions
{
    /// <summary>
    ///     The storage options.
    /// </summary>
    public RedisPersistenceStorageOptions[] Storages { get; set; } = Array.Empty<RedisPersistenceStorageOptions>();

    /// <summary>
    ///     Gets the connection strings.
    /// </summary>
    [Redact]
    public ConnectionStrings ConnectionStrings { get; internal set; } = new();

}

/// <summary>
/// </summary>
[PublicAPI]
public sealed class RedisPersistenceStorageOptions
{
    /// <summary>
    ///     The name of the connection string.
    /// </summary>
    public string ConnectionStringName { get; set; } = ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME;

    /// <summary>
    ///     Whether or not to delete state during a clear operation.
    /// </summary>
    public bool DeleteStateOnClear { get; set; }

    /// <summary>
    ///     The stage of silo lifecycle where storage should be initialized.  Storage must be initialized prior to use.
    /// </summary>
    public int InitStage { get; set; } = 10000;
}
