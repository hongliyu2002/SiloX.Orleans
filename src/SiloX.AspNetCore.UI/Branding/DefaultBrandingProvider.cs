using JetBrains.Annotations;

namespace SiloX.AspNetCore.UI.Branding;

/// <summary>
///     Default implementation of <see cref="IBrandingProvider" />.
/// </summary>
[PublicAPI]
public class DefaultBrandingProvider : IBrandingProvider
{
    /// <inheritdoc />
    public virtual string AppName => "SiloX";

    /// <inheritdoc />
    public virtual string LogoUrl => string.Empty;

    /// <inheritdoc />
    public virtual string? LogoReverseUrl => null;
}