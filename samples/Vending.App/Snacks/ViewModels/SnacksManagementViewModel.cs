using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Fluxera.Utilities.Extensions;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.FluentResults;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Vending.Projection.Abstractions.Snacks;

namespace Vending.App.ViewModels;

public class SnacksManagementViewModel : ReactiveObject
{
    private readonly IClusterClient? _clusterClient;

    /// <inheritdoc />
    public SnacksManagementViewModel()
    {
        var appLifetime = Locator.Current.GetService<IHostApplicationLifetime>();
        if (appLifetime != null)
        {
            appLifetime.ApplicationStarted.Register(() => IsInitialized = true);
            appLifetime.ApplicationStopped.Register(() => IsInitialized = false);
        }
        _clusterClient = Locator.Current.GetService<IClusterClient>();
        this.WhenAnyValue(vm => vm.SearchTerm)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .Select(term => term.Trim())
            .DistinctUntilChanged()
            .CombineLatest(this.WhenAnyValue(vm => vm.IsInitialized).Where(initialized => initialized))
            .SelectMany(termInitialized => GetSnackItemsAsync(termInitialized.First))
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToPropertyEx(this, vm => vm.SnackItems);
        this.WhenAnyValue(vm => vm.SnackItems).Select(items => items.IsNotNullOrEmpty()).ToPropertyEx(this, vm => vm.IsSnackItemsAvailable);
        AddSnackCommand = ReactiveCommand.Create(AddSnack);
        RemoveSnackCommand = ReactiveCommand.Create(RemoveSnack);
        MoveNavigationSideCommand = ReactiveCommand.Create(MoveNavigationSide);
    }

    private async Task<IEnumerable<SnackItemViewModel>> GetSnackItemsAsync(string? term)
    {
        if (!IsInitialized)
        {
            return Enumerable.Empty<SnackItemViewModel>();
        }
        var sortings = new Dictionary<string, bool> { { "CreatedAt", true } };
        var result = await Result.Ok()
                                 .Ensure(_clusterClient != null, "Cluster client is not available")
                                 .MapTry(() => _clusterClient!.GetGrain<ISnackRetrieverGrain>("Manager"))
                                 .BindTryAsync(grain => grain.SearchingListAsync(new SnackRetrieverSearchingListQuery(term, null, null, null, null, null, null, null, null, null, false, sortings, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
                                 .MapAsync(snacks => snacks.Select(snack => new SnackItemViewModel(snack)));
        return result.IsSuccess ? result.Value : Enumerable.Empty<SnackItemViewModel>();
    }

    #region Properties

    [Reactive]
    public bool IsInitialized { get; set; }

    [Reactive]
    public NavigationSide NavigationSide { get; set; }

    [Reactive]
    public string SearchTerm { get; set; } = string.Empty;

    [ObservableAsProperty]
    public IEnumerable<SnackItemViewModel>? SnackItems { get; }

    [ObservableAsProperty]
    public bool IsSnackItemsAvailable { get; }

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Unit> AddSnackCommand { get; }

    private void AddSnack()
    {
        if (!IsInitialized)
        {
        }
    }

    public ReactiveCommand<Unit, Unit> RemoveSnackCommand { get; }

    private void RemoveSnack()
    {
        if (!IsInitialized)
        {
        }
    }

    public ReactiveCommand<Unit, Unit> MoveNavigationSideCommand { get; }

    private void MoveNavigationSide()
    {
        NavigationSide = NavigationSide == NavigationSide.Left ? NavigationSide.Right : NavigationSide.Left;
    }

    #endregion

}
