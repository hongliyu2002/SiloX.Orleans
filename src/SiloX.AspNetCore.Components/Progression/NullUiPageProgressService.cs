using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Progression;

/// <summary>
///     A null implementation of <see cref="IUiPageProgressService" /> that does nothing.
/// </summary>
[PublicAPI]
public class NullUiPageProgressService : IUiPageProgressService
{
    /// <inheritdoc />
    public event EventHandler<UiPageProgressEventArgs>? ProgressChanged;

    /// <summary>
    ///     Raises the <see cref="ProgressChanged" /> event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected void OnProgressChanged(UiPageProgressEventArgs e)
    {
        ProgressChanged?.Invoke(this, e);
    }

    /// <inheritdoc />
    public Task Go(int? percentage, Action<UiPageProgressOptions>? options = null)
    {
        return Task.CompletedTask;
    }
}