using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Web.Cookies;

/// <summary>
///     A service for managing cookies.
/// </summary>
[PublicAPI]
public interface ICookieService
{
    /// <summary>
    ///     Sets a cookie.
    /// </summary>
    /// <param name="key">The name of the cookie.</param>
    /// <param name="value">The value of the cookie. If <c>null</c>, the cookie will be deleted.</param>
    /// <param name="options">Options for the cookie.</param>
    /// <returns>A <see cref="ValueTask" /> that represents the asynchronous operation.</returns>
    public ValueTask SetAsync(string key, string value, CookieOptions? options = null);

    /// <summary>
    ///     Gets a cookie.
    /// </summary>
    /// <param name="key">The name of the cookie.</param>
    /// <returns>A <see cref="ValueTask" /> that represents the asynchronous operation.</returns>
    public ValueTask<string> GetAsync(string key);

    /// <summary>
    ///     Deletes a cookie.
    /// </summary>
    /// <param name="key">The name of the cookie.</param>
    /// <param name="path">The path for which the cookie is valid. If <c>null</c>, the path will be set to the current request path.</param>
    /// <returns>A <see cref="ValueTask" /> that represents the asynchronous operation.</returns>
    public ValueTask DeleteAsync(string key, string? path = null);
}