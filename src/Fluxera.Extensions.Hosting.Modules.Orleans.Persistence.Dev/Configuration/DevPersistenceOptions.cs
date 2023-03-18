using JetBrains.Annotations;
using Orleans.Providers;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Persistence.Dev;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class DevPersistenceOptions
{
    /// <summary>
    ///     The storage descriptions.
    /// </summary>
    public StorageDescription[] Storages { get; set; } = Array.Empty<StorageDescription>();

    /// <summary>
    /// </summary>
    public class StorageDescription
    {
        /// <summary>
        ///     The name of the connection string.
        /// </summary>
        public string Name { get; set; } = ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME;

        /// <summary>
        ///     The number of store grains to use.
        /// </summary>
        public int NumStorageGrains { get; set; } = 10;

        /// <summary>
        ///     The stage of silo lifecycle where storage should be initialized.  Storage must be initialized prior to use.
        /// </summary>
        public int InitStage { get; set; } = 10000;
    }
}
