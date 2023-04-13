using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Progression;

/// <summary>
///     The type of the progress.
/// </summary>
[PublicAPI]
public enum UIPageProgressType
{
    /// <summary>
    ///     The default type.
    /// </summary>
    Default,

    /// <summary>
    ///     The info type.
    /// </summary>
    Info,

    /// <summary>
    ///     The success type.
    /// </summary>
    Success,

    /// <summary>
    ///     The warning type.
    /// </summary>
    Warning,

    /// <summary>
    ///     The error type.
    /// </summary>
    Error
}