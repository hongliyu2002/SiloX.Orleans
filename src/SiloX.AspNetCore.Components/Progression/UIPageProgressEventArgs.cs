using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Progression;

/// <summary>
///     Event arguments for the page progress event.
/// </summary>
[PublicAPI]
public class UIPageProgressEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UIPageProgressEventArgs" /> class.
    /// </summary>
    /// <param name="percentage">The percentage of the progress.</param>
    /// <param name="options">The options to override page progress appearance.</param>
    public UIPageProgressEventArgs(int? percentage, UIPageProgressOptions options)
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
    public UIPageProgressOptions Options { get; }
}