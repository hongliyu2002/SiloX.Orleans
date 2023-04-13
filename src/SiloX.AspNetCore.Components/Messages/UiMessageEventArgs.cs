using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Messages;

/// <summary>
///     Event arguments for the <see cref="IUiMessageService" />.
/// </summary>
[PublicAPI]
public class UiMessageEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UiMessageEventArgs" /> class.
    /// </summary>
    /// <param name="messageType">The type of the message.</param>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <param name="options">The options.</param>
    public UiMessageEventArgs(UiMessageType messageType, string message, string title, UiMessageOptions options)
    {
        MessageType = messageType;
        Message = message;
        Title = title;
        Options = options;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UiMessageEventArgs" /> class.
    /// </summary>
    /// <param name="messageType">The type of the message.</param>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <param name="options">The options.</param>
    /// <param name="callback">The callback.</param>
    public UiMessageEventArgs(UiMessageType messageType, string message, string title, UiMessageOptions options, TaskCompletionSource<bool>? callback)
    {
        MessageType = messageType;
        Message = message;
        Title = title;
        Options = options;
        Callback = callback;
    }

    /// <summary>
    ///     Gets the type of the message.
    /// </summary>
    public UiMessageType MessageType { get; set; }

    /// <summary>
    ///     Gets the message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    ///     Gets the title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    ///     Gets the options.
    /// </summary>
    public UiMessageOptions Options { get; }

    /// <summary>
    ///     Gets the callback.
    /// </summary>
    public TaskCompletionSource<bool>? Callback { get; }
}