using JetBrains.Annotations;

namespace SiloX.AspNetCore.UI.Branding;

/// <summary>
///     Provides branding information for the application
/// </summary>
[PublicAPI]
public interface IBrandingProvider
{
    /// <summary>
    ///     Name of the application
    /// </summary>
    string AppName { get; }

    /// <summary>
    ///     Logo on white background
    /// </summary>
    string LogoUrl { get; }

    /// <summary>
    ///     Logo on dark background
    /// </summary>
    string? LogoReverseUrl { get; }
}