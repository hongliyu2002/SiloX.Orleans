using JetBrains.Annotations;
using Microsoft.JSInterop;

namespace SiloX.AspNetCore.Components.Web.Utilities;

/// <summary>
///     A service that provides utility methods for JavaScript interop.
/// </summary>
[PublicAPI]
public class UtilsService : IUtilsService
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UtilsService" /> class.
    /// </summary>
    /// <param name="jsRuntime">The <see cref="IJSRuntime" /> instance to use for JavaScript interop.</param>
    public UtilsService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    /// <summary>
    ///     Gets the <see cref="IJSRuntime" /> instance to use for JavaScript interop.
    /// </summary>
    protected IJSRuntime JsRuntime { get; }

    /// <inheritdoc />
    public ValueTask AddClassToTagAsync(string tagName, string className)
    {
        return JsRuntime.InvokeVoidAsync("silox.utils.addClassToTag", tagName, className);
    }

    /// <inheritdoc />
    public ValueTask RemoveClassFromTagAsync(string tagName, string className)
    {
        return JsRuntime.InvokeVoidAsync("silox.utils.removeClassFromTag", tagName, className);
    }

    /// <inheritdoc />
    public ValueTask<bool> HasClassOnTagAsync(string tagName, string className)
    {
        return JsRuntime.InvokeAsync<bool>("silox.utils.hasClassOnTag", tagName, className);
    }

    /// <inheritdoc />
    public ValueTask ReplaceLinkHrefByIdAsync(string linkId, string hrefValue)
    {
        return JsRuntime.InvokeVoidAsync("silox.utils.replaceLinkHrefById", linkId, hrefValue);
    }

    /// <inheritdoc />
    public ValueTask ToggleFullscreenAsync()
    {
        return JsRuntime.InvokeVoidAsync("silox.utils.toggleFullscreen");
    }

    /// <inheritdoc />
    public ValueTask RequestFullscreenAsync()
    {
        return JsRuntime.InvokeVoidAsync("silox.utils.requestFullscreen");
    }

    /// <inheritdoc />
    public ValueTask ExitFullscreenAsync()
    {
        return JsRuntime.InvokeVoidAsync("silox.utils.exitFullscreen");
    }
}