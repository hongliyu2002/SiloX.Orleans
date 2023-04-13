using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.ExceptionHandling;

/// <summary>
///     The context for <see cref="IUserExceptionInformer" />.
/// </summary>
[PublicAPI]
public class UserExceptionInformerContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UserExceptionInformerContext" /> class.
    /// </summary>
    /// <param name="exception">The exception.</param>
    public UserExceptionInformerContext(Exception exception)
    {
        Exception = exception;
    }

    /// <summary>
    ///     Gets the exception.
    /// </summary>
    public Exception Exception { get; }
}