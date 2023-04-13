using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SiloX.AspNetCore.Components.Alerts;
using SiloX.AspNetCore.Components.ExceptionHandling;
using SiloX.AspNetCore.Components.Messages;
using SiloX.AspNetCore.Components.Notifications;

namespace SiloX.AspNetCore.Components;

/// <summary>
/// </summary>
[PublicAPI]
public abstract class ComponentBase : OwningComponentBase
{
    // private IStringLocalizerFactory? _stringLocalizerFactory;
    // /// <summary>
    // /// </summary>
    // protected IStringLocalizerFactory StringLocalizerFactory => LazyGetRequiredService(ref _stringLocalizerFactory);
    //
    // private IStringLocalizer? _localizer;
    // /// <summary>
    // /// </summary>
    // protected IStringLocalizer Localizer => _localizer ?? (_localizer = CreateLocalizer());
    //
    // private Type _localizationResource = typeof(DefaultResource);
    // /// <summary>
    // /// </summary>
    // protected Type LocalizationResource
    // {
    //     get => _localizationResource;
    //     set
    //     {
    //         _localizationResource = value;
    //         _localizer = null;
    //     }
    // }
    //
    // /// <summary>
    // ///  </summary>
    // /// <returns></returns>
    // /// <exception cref="AbpException"></exception>
    // protected virtual IStringLocalizer CreateLocalizer()
    // {
    //     if (LocalizationResource != null)
    //     {
    //         return StringLocalizerFactory.Create(LocalizationResource);
    //     }
    //     var localizer = StringLocalizerFactory.CreateDefaultOrNull();
    //     if (localizer == null)
    //     {
    //         throw new
    //             AbpException($"Set {nameof(LocalizationResource)} or define the default localization resource type (by configuring the {nameof(AbpLocalizationOptions)}.{nameof(AbpLocalizationOptions.DefaultResourceType)}) to be able to use the {nameof(L)} object!");
    //     }
    //     return localizer;
    // }

    private Lazy<ILogger> _lazyLogger => new(() => LoggerFactory.CreateLogger(GetType().FullName!), true);
    /// <summary>
    /// </summary>
    protected ILogger Logger => _lazyLogger.Value;

    private ILoggerFactory? _loggerFactory;
    /// <summary>
    /// </summary>
    protected ILoggerFactory LoggerFactory => LazyGetRequiredService(ref _loggerFactory);

    private IAuthorizationService? _authorizationService;
    /// <summary>
    /// </summary>
    protected IAuthorizationService AuthorizationService => LazyGetRequiredService(ref _authorizationService);

    private IUiMessageService? _message;
    /// <summary>
    /// </summary>
    protected IUiMessageService Message => LazyGetNonScopedRequiredService(ref _message);

    private IUiNotificationService? _notify;
    /// <summary>
    /// </summary>
    protected IUiNotificationService Notify => LazyGetNonScopedRequiredService(ref _notify);

    private IUserExceptionInformer? _exceptionInformer;
    /// <summary>
    /// </summary>
    protected IUserExceptionInformer Informer => LazyGetNonScopedRequiredService(ref _exceptionInformer);

    private IAlertManager? _alertManager;
    /// <summary>
    /// </summary>
    protected IAlertManager AlertManager => LazyGetNonScopedRequiredService(ref _alertManager);

    /// <summary>
    /// </summary>
    /// <param name="exception"></param>
    protected virtual async Task HandleErrorAsync(Exception exception)
    {
        Logger.Log(LogLevel.Error, exception, "Error occurred in {Message}", exception.Message);
        await InvokeAsync(async () =>
                          {
                              await Informer.InformAsync(new UserExceptionInformerContext(exception));
                              StateHasChanged();
                          });
    }

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
    protected TService LazyGetNonScopedRequiredService<TService>(ref TService? reference)
    {
        return LazyGetNonScopedRequiredService(typeof(TService), ref reference);
    }

    /// <summary>
    /// </summary>
    /// <param name="serviceType"></param>
    /// <param name="reference"></param>
    /// <typeparam name="TRef"></typeparam>
    /// <returns></returns>
    protected TRef LazyGetNonScopedRequiredService<TRef>(Type serviceType, ref TRef? reference)
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