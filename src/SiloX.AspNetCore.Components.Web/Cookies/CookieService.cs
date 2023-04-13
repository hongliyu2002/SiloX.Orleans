using JetBrains.Annotations;
using Microsoft.JSInterop;

namespace SiloX.AspNetCore.Components.Web.Cookies;

/// <summary>
///     A service for managing cookies.
/// </summary>
[PublicAPI]
public class CookieService : ICookieService
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CookieService" /> class.
    /// </summary>
    /// <param name="jsRuntime">The <see cref="IJSRuntime" /> instance to use for JavaScript interop.</param>
    public CookieService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    /// <summary>
    ///     Gets the <see cref="IJSRuntime" /> instance to use for JavaScript interop.
    /// </summary>
    public IJSRuntime JsRuntime { get; }

    /// <inheritdoc />
    public ValueTask SetAsync(string key, string value, CookieOptions? options = null)
    {
        return JsRuntime.InvokeVoidAsync("silox.utils.setCookieValue", key, value, options?.ExpireDate?.ToString("r"), options?.Path, options?.Secure);
    }

    /// <inheritdoc />
    public ValueTask<string> GetAsync(string key)
    {
        return JsRuntime.InvokeAsync<string>("silox.utils.getCookieValue", key);
    }

    /// <inheritdoc />
    public ValueTask DeleteAsync(string key, string? path = null)
    {
        return JsRuntime.InvokeVoidAsync("silox.utils.deleteCookie", key);
    }
}