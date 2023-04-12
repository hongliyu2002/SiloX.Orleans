namespace SiloX.AspNetCore.Components.Alerts;

/// <summary>
///     Interface for the AlertManager service.
/// </summary>
public interface IAlertManager
{
    /// <summary>
    ///     Gets the list of alerts.
    /// </summary>
    AlertList Alerts { get; }
}