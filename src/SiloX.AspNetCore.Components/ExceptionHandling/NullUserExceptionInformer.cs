using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.ExceptionHandling;

/// <summary>
///     A null implementation of <see cref="IUserExceptionInformer" /> that does nothing.
/// </summary>
[PublicAPI]
public class NullUserExceptionInformer : IUserExceptionInformer
{
    /// <summary>
    ///     Does nothing.
    /// </summary>
    /// <param name="context"></param>
    public void Inform(UserExceptionInformerContext context)
    {
    }

    /// <summary>
    ///     Does nothing.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task InformAsync(UserExceptionInformerContext context)
    {
        return Task.CompletedTask;
    }
}