using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Notifications;

/// <summary>
///     A null implementation of <see cref="IUiNotificationService" /> that does nothing.
/// </summary>
[PublicAPI]
public class NullUiNotificationService : IUiNotificationService
{
    /// <inheritdoc />
    public Task Info(string message, string? title = null, Action<UiNotificationOptions>? options = null)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task Success(string message, string? title = null, Action<UiNotificationOptions>? options = null)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task Warn(string message, string? title = null, Action<UiNotificationOptions>? options = null)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task Error(string message, string? title = null, Action<UiNotificationOptions>? options = null)
    {
        return Task.CompletedTask;
    }
}