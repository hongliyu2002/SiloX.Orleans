using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Alerts;

/// <summary>
///     A list of alerts.
/// </summary>
[PublicAPI]
public class AlertList : ObservableCollection<AlertMessage>
{
    /// <summary>
    ///     Adds an alert to the list.
    /// </summary>
    public void Add(AlertType type, string text, string? title = null, bool dismissible = true)
    {
        Add(new AlertMessage(type, text, title, dismissible));
    }

    /// <summary>
    ///     Adds an alert of info to the list.
    /// </summary>
    public void Info(string text, string? title = null, bool dismissible = true)
    {
        Add(new AlertMessage(AlertType.Info, text, title, dismissible));
    }

    /// <summary>
    ///     Adds an alert of warning to the list.
    /// </summary>
    public void Warning(string text, string? title = null, bool dismissible = true)
    {
        Add(new AlertMessage(AlertType.Warning, text, title, dismissible));
    }

    /// <summary>
    ///     Adds an alert of danger to the list.
    /// </summary>
    public void Danger(string text, string? title = null, bool dismissible = true)
    {
        Add(new AlertMessage(AlertType.Danger, text, title, dismissible));
    }

    /// <summary>
    ///     Adds an alert of success to the list.
    /// </summary>
    public void Success(string text, string? title = null, bool dismissible = true)
    {
        Add(new AlertMessage(AlertType.Success, text, title, dismissible));
    }
}