using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Alerts;

/// <summary>
///     Interface for the alert manager service.
/// </summary>
[PublicAPI]
public interface IAlertManager
{
    /// <summary>
    ///     Gets the list of alerts.
    /// </summary>
    AlertList Alerts { get; }
}