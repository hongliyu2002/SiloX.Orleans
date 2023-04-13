using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Progression;

/// <summary>
///     A null implementation of <see cref="IUIPageProgressService" /> that does nothing.
/// </summary>
[PublicAPI]
public class NullIUIPageProgressService : IUIPageProgressService
{
    /// <inheritdoc />
    public event EventHandler<UIPageProgressEventArgs>? ProgressChanged;

    /// <summary>
    ///     Raises the <see cref="ProgressChanged" /> event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected void OnProgressChanged(UIPageProgressEventArgs e)
    {
        ProgressChanged?.Invoke(this, e);
    }

    /// <inheritdoc />
    public Task Go(int? percentage, Action<UIPageProgressOptions>? options = null)
    {
        return Task.CompletedTask;
    }
}