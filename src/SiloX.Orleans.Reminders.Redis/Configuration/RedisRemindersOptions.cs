using Fluxera.Extensions.DataManagement;
using JetBrains.Annotations;

namespace SiloX.Orleans.Reminders.Redis;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class RedisRemindersOptions
{
    /// <summary>
    ///     Is this configuration intended for client-side use?
    /// </summary>
    public bool UsedByClient { get; set; }

    /// <summary>
    ///     The name of the provider (also used as connection string name).
    /// </summary>
    public string ProviderName { get; set; } = "RedisReminders";

    /// <summary>
    ///     Gets the connection strings.
    /// </summary>
    [Redact]
    public ConnectionStrings ConnectionStrings { get; internal set; } = new();
}
