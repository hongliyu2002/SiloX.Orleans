using Fluxera.Guards;
using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Messages;

/// <summary>
///     Event arguments for the <see cref="IUIMessageService" />.
/// </summary>
[PublicAPI]
public class UIMessageEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UIMessageEventArgs" /> class.
    /// </summary>
    /// <param name="messageType">The type of the message.</param>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <param name="options">The options.</param>
    public UIMessageEventArgs(UIMessageType messageType, string message, string title, UIMessageOptions options)
    {
        MessageType = messageType;
        Message = Guard.Against.NullOrEmpty(message, nameof(message));
        Title = Guard.Against.NullOrEmpty(title, nameof(title));
        Options = Guard.Against.Null(options, nameof(options));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UIMessageEventArgs" /> class.
    /// </summary>
    /// <param name="messageType">The type of the message.</param>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <param name="options">The options.</param>
    /// <param name="callback">The callback.</param>
    public UIMessageEventArgs(UIMessageType messageType, string message, string title, UIMessageOptions options, TaskCompletionSource<bool>? callback)
        : this(messageType, message, title, options)
    {
        Callback = callback;
    }

    /// <summary>
    ///     Gets the type of the message.
    /// </summary>
    public UIMessageType MessageType { get; set; }

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
    public UIMessageOptions Options { get; }

    /// <summary>
    ///     Gets the callback.
    /// </summary>
    public TaskCompletionSource<bool>? Callback { get; }
}