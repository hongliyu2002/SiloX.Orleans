using JetBrains.Annotations;

namespace SiloX.UI.LayoutHooks;

/// <summary>
///     Layout hook information.
/// </summary>
[PublicAPI]
public class LayoutHookInfo
{
    /// <summary>
    /// </summary>
    /// <param name="componentType"></param>
    /// <param name="layout"></param>
    public LayoutHookInfo(Type componentType, string? layout = null)
    {
        ComponentType = componentType;
        Layout = layout;
    }

    /// <summary>
    ///     Component type.
    /// </summary>
    public Type ComponentType { get; }

    /// <summary>
    ///     Specifies the layout name to apply this hook.
    ///     null indicates that this hook will be applied to all layouts.
    /// </summary>
    public string? Layout { get; }
}