using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Notifications;

/// <summary>
///     The type of notification.
/// </summary>
[PublicAPI]
public enum UINotificationType
{
    /// <summary>
    ///     The notification is informational.
    /// </summary>
    Info,

    /// <summary>
    ///     The notification is a success message.
    /// </summary>
    Success,

    /// <summary>
    ///     The notification is a warning.
    /// </summary>
    Warning,

    /// <summary>
    ///     The notification is an error.
    /// </summary>
    Error
}