using JetBrains.Annotations;

namespace Fluxera.Extensions.Hosting.Modules.Orleans.Reminder;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class ReminderOptions
{
    /// <summary>
    ///     The minimum period for reminders.
    /// </summary>
    /// <remarks>
    ///     High-frequency reminders are dangerous for production systems.
    /// </remarks>
    public TimeSpan MinimumReminderPeriod { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    ///     The period between reminder table refreshes.
    /// </summary>
    /// <value>Refresh the reminder table every 5 minutes by default.</value>
    public TimeSpan RefreshReminderListPeriod { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    ///     The maximum amount of time to attempt to initialize reminders before giving up.
    /// </summary>
    /// <value>Attempt to initialize for 5 minutes before giving up by default.</value>
    public TimeSpan InitializationTimeout { get; set; } = TimeSpan.FromMinutes(5);
}
