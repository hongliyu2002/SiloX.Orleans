using System.Text.Json.Serialization;
using Fluxera.Extensions.DataManagement;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Vending.Projection.EntityFrameworkCore;

/// <summary>
///     Represents the configuration options for Entity Framework Core.
/// </summary>
[PublicAPI]
public sealed class ProjectionEFCoreOptions
{
    /// <summary>
    ///     The name of the provider (also used as connection string name).
    /// </summary>
    public string ProviderName { get; set; } = "Vending-Projection";

    /// <summary>
    ///     The table name for the migrations history table.
    /// </summary>
    public string? MigrationsHistoryTable { get; set; }

    /// <summary>
    ///     The query splitting behavior for Entity Framework Core.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public QuerySplittingBehavior QuerySplittingBehavior { get; set; } = QuerySplittingBehavior.SingleQuery;

    /// <summary>
    ///     The maximum number of times to retry a failed operation.
    /// </summary>
    public int MaxRetry { get; set; } = 3;

    /// <summary>
    ///     The maximum delay between retries.
    /// </summary>
    public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    ///     Gets the connection strings.
    /// </summary>
    [Redact]
    public ConnectionStrings ConnectionStrings { get; internal set; } = new();
}
