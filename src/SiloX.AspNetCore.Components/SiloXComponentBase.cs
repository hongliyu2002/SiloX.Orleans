using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SiloX.AspNetCore.Components;

/// <summary>
/// </summary>
[PublicAPI]
public abstract class SiloXComponentBase : OwningComponentBase
{
    /// <summary>
    /// </summary>
    protected ILogger Logger => _lazyLogger.Value;
    private Lazy<ILogger> _lazyLogger => new(() => LoggerFactory.CreateLogger(GetType().FullName!), true);

    /// <summary>
    /// </summary>
    protected ILoggerFactory LoggerFactory => LazyGetRequiredService(ref _loggerFactory);
    private ILoggerFactory? _loggerFactory;
    
    /// <summary>
    /// </summary>
    protected IAuthorizationService AuthorizationService => LazyGetRequiredService(ref _authorizationService);
    private IAuthorizationService? _authorizationService;

    #region Get Service

    /// <summary>
    /// </summary>
    [Inject]
    protected IServiceProvider NonScopedServices { get; set; } = null!;

    /// <summary>
    /// </summary>
    /// <param name="reference"></param>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    protected TService LazyGetRequiredService<TService>(ref TService? reference)
    {
        return LazyGetRequiredService(typeof(TService), ref reference);
    }

    /// <summary>
    /// </summary>
    /// <param name="serviceType"></param>
    /// <param name="reference"></param>
    /// <typeparam name="TRef"></typeparam>
    /// <returns></returns>
    protected TRef LazyGetRequiredService<TRef>(Type serviceType, ref TRef? reference)
    {
        if (reference is null)
        {
            reference = (TRef)ScopedServices.GetRequiredService(serviceType);
        }
        return reference;
    }

    /// <summary>
    /// </summary>
    /// <param name="reference"></param>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    protected TService? LazyGetService<TService>(ref TService? reference)
    {
        return LazyGetService(typeof(TService), ref reference);
    }

    /// <summary>
    /// </summary>
    /// <param name="serviceType"></param>
    /// <param name="reference"></param>
    /// <typeparam name="TRef"></typeparam>
    /// <returns></returns>
    protected TRef? LazyGetService<TRef>(Type serviceType, ref TRef? reference)
    {
        if (reference is null)
        {
            reference = (TRef?)ScopedServices.GetService(serviceType);
        }
        return reference;
    }

    /// <summary>
    /// </summary>
    /// <param name="reference"></param>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    protected TService LazyGetNonScopedRequiredService<TService>(ref TService reference)
    {
        return LazyGetNonScopedRequiredService(typeof(TService), ref reference);
    }

    /// <summary>
    /// </summary>
    /// <param name="serviceType"></param>
    /// <param name="reference"></param>
    /// <typeparam name="TRef"></typeparam>
    /// <returns></returns>
    protected TRef LazyGetNonScopedRequiredService<TRef>(Type serviceType, ref TRef reference)
    {
        if (reference is null)
        {
            reference = (TRef)NonScopedServices.GetRequiredService(serviceType);
        }
        return reference;
    }

    /// <summary>
    /// </summary>
    /// <param name="reference"></param>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    protected TService? LazyGetNonScopedService<TService>(ref TService? reference)
    {
        return LazyGetNonScopedService(typeof(TService), ref reference);
    }

    /// <summary>
    /// </summary>
    /// <param name="serviceType"></param>
    /// <param name="reference"></param>
    /// <typeparam name="TRef"></typeparam>
    /// <returns></returns>
    protected TRef? LazyGetNonScopedService<TRef>(Type serviceType, ref TRef? reference)
    {
        if (reference is null)
        {
            reference = (TRef?)NonScopedServices.GetService(serviceType);
        }
        return reference;
    }

    #endregion

}