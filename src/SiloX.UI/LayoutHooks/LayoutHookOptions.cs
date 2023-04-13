using Fluxera.Utilities.Extensions;
using JetBrains.Annotations;

namespace SiloX.UI.LayoutHooks;

/// <summary>
///     Layout hook options
/// </summary>
[PublicAPI]
public class LayoutHookOptions
{

    /// <summary>
    /// </summary>
    public LayoutHookOptions()
    {
        Hooks = new Dictionary<string, List<LayoutHookInfo>>();
    }

    /// <summary>
    /// </summary>
    public IDictionary<string, List<LayoutHookInfo>> Hooks { get; }

    /// <summary>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="componentType"></param>
    /// <param name="layout"></param>
    /// <returns></returns>
    public LayoutHookOptions Add(string name, Type componentType, string? layout = null)
    {
        Hooks.GetOrAdd(name, () => new List<LayoutHookInfo>()).Add(new LayoutHookInfo(componentType, layout));
        return this;
    }
}