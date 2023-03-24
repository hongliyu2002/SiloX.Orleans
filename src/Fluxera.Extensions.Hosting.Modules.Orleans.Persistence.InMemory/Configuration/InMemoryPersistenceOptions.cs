using JetBrains.Annotations;
using Orleans.Providers;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.InMemory;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class InMemoryPersistenceOptions
{
    /// <summary>
    ///     The storage options.
    /// </summary>
    public InMemoryPersistenceStorageOptions[] StorageOptions { get; set; } = Array.Empty<InMemoryPersistenceStorageOptions>();
}

/// <summary>
/// </summary>
[PublicAPI]
public sealed class InMemoryPersistenceStorageOptions
{
    /// <summary>
    ///     The name of the connection.
    /// </summary>
    public string Name { get; set; } = ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME;

    /// <summary>
    ///     The number of store grains to use.
    /// </summary>
    public int NumStorageGrains { get; set; } = 10;

    /// <summary>
    ///     The stage of silo lifecycle where storage should be initialized.  Storage must be initialized prior to use.
    /// </summary>
    public int InitStage { get; set; } = ServiceLifecycleStage.ApplicationServices;
}
