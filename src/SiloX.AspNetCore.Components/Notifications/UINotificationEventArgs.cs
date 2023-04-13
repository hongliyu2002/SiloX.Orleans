using Fluxera.Guards;
using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Notifications;

/// <summary>
///     Event args for the <see cref="IUINotificationService" /> event.
/// </summary>
[PublicAPI]
public class UINotificationEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UINotificationEventArgs" /> class.
    /// </summary>
    /// <param name="notificationType">The type of the notification.</param>
    /// <param name="message">The message of the notification.</param>
    /// <param name="title">The title of the notification.</param>
    /// <param name="options">The options of the notification.</param>
    public UINotificationEventArgs(UINotificationType notificationType, string message, string title, UINotificationOptions options)
    {
        NotificationType = notificationType;
        Message = Guard.Against.NullOrEmpty(message, nameof(message));
        Title = Guard.Against.NullOrEmpty(title, nameof(title));
        Options = Guard.Against.Null(options, nameof(options));
    }

    /// <summary>
    ///     Gets the type of the notification.
    /// </summary>
    public UINotificationType NotificationType { get; set; }

    /// <summary>
    ///     Gets the message of the notification.
    /// </summary>
    public string Message { get; }

    /// <summary>
    ///     Gets the title of the notification.
    /// </summary>
    public string Title { get; }

    /// <summary>
    ///     Gets the options of the notification.
    /// </summary>
    public UINotificationOptions Options { get; }
}