using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Notifications;

/// <summary>
///     A service to show notifications to the user.
/// </summary>
[PublicAPI]
public interface IUiNotificationService
{
    /// <summary>
    ///     Show a notification to the user.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The title of the notification.</param>
    /// <param name="options">An action to configure the notification.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    Task Info(string message, string? title = null, Action<UiNotificationOptions>? options = null);

    /// <summary>
    ///     Show a success notification to the user.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The title of the notification.</param>
    /// <param name="options">An action to configure the notification.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    Task Success(string message, string? title = null, Action<UiNotificationOptions>? options = null);

    /// <summary>
    ///     Show a warning notification to the user.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The title of the notification.</param>
    /// <param name="options">An action to configure the notification.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    Task Warn(string message, string? title = null, Action<UiNotificationOptions>? options = null);

    /// <summary>
    ///     Show an error notification to the user.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The title of the notification.</param>
    /// <param name="options">An action to configure the notification.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    Task Error(string message, string? title = null, Action<UiNotificationOptions>? options = null);
}