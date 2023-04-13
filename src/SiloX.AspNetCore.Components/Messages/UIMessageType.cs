using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Messages;

/// <summary>
///     Defines the possible ui message types with predefined actions.
/// </summary>
[PublicAPI]
public enum UIMessageType
{
    /// <summary>
    ///     The message is informational.
    /// </summary>
    Info,

    /// <summary>
    ///     The message is a success message.
    /// </summary>
    Success,

    /// <summary>
    ///     The message is a warning message.
    /// </summary>
    Warning,

    /// <summary>
    ///     The message is an error message.
    /// </summary>
    Error,

    /// <summary>
    ///     The message is a confirmation message.
    /// </summary>
    Confirmation
}