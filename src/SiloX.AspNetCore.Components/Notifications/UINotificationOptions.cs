using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Notifications;

/// <summary>
///     Options to override notification appearance.
/// </summary>
[PublicAPI]
public class UINotificationOptions
{
    /// <summary>
    ///     Custom text for the Ok button.
    /// </summary>
    public string? OkButtonText { get; set; }

    /// <summary>
    ///     Custom icon for the Ok button.
    /// </summary>
    public object? OkButtonIcon { get; set; }
}