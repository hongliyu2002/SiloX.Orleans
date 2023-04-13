namespace SiloX.AspNetCore.Components.ExceptionHandling;

/// <summary>
///     Interface for user exception informers.
/// </summary>
public interface IUserExceptionInformer
{
    /// <summary>
    ///     Inform the user about the exception.
    /// </summary>
    /// <param name="context">The context.</param>
    void Inform(UserExceptionInformerContext context);

    /// <summary>
    ///     Inform the user about the exception.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    Task InformAsync(UserExceptionInformerContext context);
}