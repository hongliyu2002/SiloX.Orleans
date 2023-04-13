using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Notifications;

/// <summary>
///     Event args for the <see cref="IUiNotificationService" /> event.
/// </summary>
[PublicAPI]
public class UiNotificationEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UiNotificationEventArgs" /> class.
    /// </summary>
    /// <param name="notificationType">The type of the notification.</param>
    /// <param name="message">The message of the notification.</param>
    /// <param name="title">The title of the notification.</param>
    /// <param name="options">The options of the notification.</param>
    public UiNotificationEventArgs(UiNotificationType notificationType, string message, string title, UiNotificationOptions options)
    {
        NotificationType = notificationType;
        Message = message;
        Title = title;
        Options = options;
    }

    /// <summary>
    ///     Gets the type of the notification.
    /// </summary>
    public UiNotificationType NotificationType { get; set; }

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
    public UiNotificationOptions Options { get; }
}