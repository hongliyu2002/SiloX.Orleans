using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Progression;

/// <summary>
///     Options to override page progress appearance.
/// </summary>
[PublicAPI]
public class UIPageProgressOptions
{
    /// <summary>
    ///     Type or color, of the page progress.
    /// </summary>
    public UIPageProgressType Type { get; set; }
}