using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Progression;

/// <summary>
///     Service to control the progress bar on the page.
/// </summary>
[PublicAPI]
public interface IUiPageProgressService
{
    /// <summary>
    ///     An event raised after the notification is received.
    /// </summary>
    public event EventHandler<UiPageProgressEventArgs>? ProgressChanged;

    /// <summary>
    ///     Sets the progress percentage.
    /// </summary>
    /// <param name="percentage">Value of the progress from 0 to 100, or null for indeterminate progress.</param>
    /// <param name="options">Additional options.</param>
    /// <returns>Awaitable task.</returns>
    Task Go(int? percentage, Action<UiPageProgressOptions>? options = null);
}