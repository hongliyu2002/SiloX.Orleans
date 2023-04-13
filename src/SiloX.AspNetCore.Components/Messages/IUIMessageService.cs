using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Messages;

/// <summary>
///     A service to show messages to the user.
/// </summary>
[PublicAPI]
public interface IUIMessageService
{
    /// <summary>
    ///     Show a message to the user.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The title of the message.</param>
    /// <param name="options">The options to use when showing the message.</param>
    /// <returns>A task that completes when the message has been shown.</returns>
    Task Info(string message, string? title = null, Action<UIMessageOptions>? options = null);

    /// <summary>
    ///     Show a success message to the user.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The title of the message.</param>
    /// <param name="options">The options to use when showing the message.</param>
    /// <returns>A task that completes when the message has been shown.</returns>
    Task Success(string message, string? title = null, Action<UIMessageOptions>? options = null);

    /// <summary>
    ///     Show a warning message to the user.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The title of the message.</param>
    /// <param name="options">The options to use when showing the message.</param>
    /// <returns>A task that completes when the message has been shown.</returns>
    Task Warn(string message, string? title = null, Action<UIMessageOptions>? options = null);

    /// <summary>
    ///     Show an error message to the user.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The title of the message.</param>
    /// <param name="options">The options to use when showing the message.</param>
    /// <returns>A task that completes when the message has been shown.</returns>
    Task Error(string message, string? title = null, Action<UIMessageOptions>? options = null);

    /// <summary>
    ///     Show a confirmation message to the user.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">The title of the message.</param>
    /// <param name="options">The options to use when showing the message.</param>
    /// <returns>A task that completes when the message has been shown.</returns>
    Task<bool> Confirm(string message, string? title = null, Action<UIMessageOptions>? options = null);
}