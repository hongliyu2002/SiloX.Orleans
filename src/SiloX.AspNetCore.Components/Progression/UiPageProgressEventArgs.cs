using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Progression;

/// <summary>
///     Event arguments for the page progress event.
/// </summary>
[PublicAPI]
public class UiPageProgressEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UiPageProgressEventArgs" /> class.
    /// </summary>
    /// <param name="percentage">The percentage of the progress.</param>
    /// <param name="options">The options to override page progress appearance.</param>
    public UiPageProgressEventArgs(int? percentage, UiPageProgressOptions options)
    {
        Percentage = percentage;
        Options = options;
    }

    /// <summary>
    ///     The percentage of the progress.
    /// </summary>
    public int? Percentage { get; }

    /// <summary>
    ///     The options to override page progress appearance.
    /// </summary>
    public UiPageProgressOptions Options { get; }
}