using Fluxera.Guards;
using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Alerts;

/// <summary>
///     Represents an alert message.
/// </summary>
[PublicAPI]
public class AlertMessage
{
    /// <summary>
    ///     Creates a new instance of <see cref="AlertMessage" />.
    /// </summary>
    /// <param name="type">The type of the alert message.</param>
    /// <param name="text">The text of the alert message.</param>
    /// <param name="title">The title of the alert message.</param>
    /// <param name="dismissible">Does the alert message have a close button?</param>
    public AlertMessage(AlertType type, string text, string? title = null, bool dismissible = true)
    {
        Type = type;
        Text = Guard.Against.NullOrWhiteSpace(text, nameof(text));
        Title = title;
        Dismissible = dismissible;
    }

    private string _text = string.Empty;
    /// <summary>
    ///     The text of the alert message.
    /// </summary>
    public string Text
    {
        get => _text;
        set => _text = Guard.Against.NullOrWhiteSpace(value, nameof(value));
    }

    /// <summary>
    ///     The type of the alert message.
    /// </summary>
    public AlertType Type { get; set; }

    /// <summary>
    ///     The title of the alert message.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    ///     Does the alert message have a close button?
    /// </summary>
    public bool Dismissible { get; set; }

}