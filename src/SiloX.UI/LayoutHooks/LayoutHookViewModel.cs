using JetBrains.Annotations;

namespace SiloX.UI.LayoutHooks;

/// <summary>
///     View model for the layout hook view.
/// </summary>
[PublicAPI]
public class LayoutHookViewModel
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="LayoutHookViewModel" /> class.
    /// </summary>
    /// <param name="hooks">The layout hooks.</param>
    /// <param name="layout">The layout.</param>
    public LayoutHookViewModel(LayoutHookInfo[] hooks, string layout)
    {
        Hooks = hooks;
        Layout = layout;
    }

    /// <summary>
    ///     Gets the layout hooks.
    /// </summary>
    public LayoutHookInfo[] Hooks { get; }

    /// <summary>
    ///     Gets the layout.
    /// </summary>
    public string Layout { get; }

}