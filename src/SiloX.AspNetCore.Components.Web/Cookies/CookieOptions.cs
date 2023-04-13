using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Web.Cookies;

/// <summary>
///     Options for a cookie.
/// </summary>
[PublicAPI]
public class CookieOptions
{
    /// <summary>
    ///     The domain for which the cookie is valid.
    /// </summary>
    public DateTimeOffset? ExpireDate { get; set; }

    /// <summary>
    ///     The path for which the cookie is valid.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    ///     Does the cookie require a secure connection?
    /// </summary>
    public bool Secure { get; set; }
}