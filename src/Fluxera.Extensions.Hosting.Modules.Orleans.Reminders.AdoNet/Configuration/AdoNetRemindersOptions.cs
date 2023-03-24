using System.Text.Json.Serialization;
using Fluxera.Extensions.DataManagement;
using JetBrains.Annotations;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminders.AdoNet;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class AdoNetRemindersOptions
{
    /// <summary>
    ///     Is this configuration intended for client-side use?
    /// </summary>
    public bool UsedByClient { get; set; }

    /// <summary>
    ///     The name of the connection string.
    /// </summary>
    public string ConnectionStringName { get; set; } = "AdoNetReminders";

    /// <summary>
    ///     The name and database provider type of the connection string.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AdoNetDbProvider DbProvider { get; set; } = AdoNetDbProvider.SQLServer;

    /// <summary>
    ///     The stage of silo lifecycle where storage should be initialized.  Storage must be initialized prior to use.
    /// </summary>
    public int InitStage { get; set; } = ServiceLifecycleStage.ApplicationServices;

    /// <summary>
    ///     Gets the connection strings.
    /// </summary>
    [Redact]
    public ConnectionStrings ConnectionStrings { get; internal set; } = new();
}
