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
using Vending.Domain.Abstractions.Snacks;
using Vending.Projection.Abstractions.Snacks;
using Snack = Vending.Projection.Abstractions.Snacks.Snack;

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
            appLifetime.ApplicationStarted.Register(() => Initialized = true);
            appLifetime.ApplicationStopped.Register(() => Initialized = false);
        }
        _clusterClient = Locator.Current.GetService<IClusterClient>();
        this.WhenAnyValue(vm => vm.SearchTerm, vm => vm.Initialized)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .Where(pair => pair.Item2)
            .Select(pair => pair.Item1.Trim())
            .DistinctUntilChanged()
            .SelectMany(GetSnackItemsAsync)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToPropertyEx(this, vm => vm.SnackItems);
        this.WhenAnyValue(vm => vm.SnackItems).Select(items => items.IsNotNullOrEmpty()).ToPropertyEx(this, vm => vm.SnackItemsAvailable);
        this.WhenAnyValue(vm => vm.SelectedSnackItem, vm => vm.Initialized)
            .Where(pair => pair.Item2)
            .Select(pair => pair.Item1?.Id ?? Guid.Empty)
            .DistinctUntilChanged()
            .SelectMany(GetSnackEditAsync)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToPropertyEx(this, vm => vm.SelectedSnackEdit);
        AddSnackCommand = ReactiveCommand.Create(AddSnack);
        RemoveSnackCommand = ReactiveCommand.Create(RemoveSnack);
        MoveNavigationSideCommand = ReactiveCommand.Create(MoveNavigationSide);
    }

    private async Task<IEnumerable<SnackItemViewModel>> GetSnackItemsAsync(string? searchTerm)
    {
        if (!Initialized)
        {
            return Enumerable.Empty<SnackItemViewModel>();
        }
        var sortings = new Dictionary<string, bool> { { "CreatedAt", true } };
        var result = await Result.Ok()
                                 .Ensure(_clusterClient != null, "Cluster client is not available")
                                 .MapTry(() => _clusterClient!.GetGrain<ISnackRetrieverGrain>("Manager"))
                                 .BindTryAsync(grain => grain.SearchingListAsync(new SnackRetrieverSearchingListQuery(searchTerm, null, null, null, null, null,
                                                                                                                      null, null, null, null, false, sortings,
                                                                                                                      Guid.NewGuid(), DateTimeOffset.UtcNow,
                                                                                                                      "Manager")))
                                 .MapAsync(snacks => snacks.Select(snack => new SnackItemViewModel(snack)));
        return result.IsSuccess ? result.Value : Enumerable.Empty<SnackItemViewModel>();
    }

    private async Task<SnackEditViewModel?> GetSnackEditAsync(Guid id)
    {
        if (!Initialized)
        {
            return null;
        }
        var result = await Result.Ok()
                                 .Ensure(_clusterClient != null, "Cluster client is not available")
                                 .MapTry(() => _clusterClient!.GetGrain<ISnackGrain>(id))
                                 .MapTryAsync(grain => grain.GetStateAsync());
        return result.IsSuccess ? new SnackEditViewModel(result.Value) : null;
    }

    #region Properties

    [Reactive]
    public bool Initialized { get; set; }

    [Reactive]
    public NavigationSide NavigationSide { get; set; }

    [Reactive]
    public string SearchTerm { get; set; } = string.Empty;

    [ObservableAsProperty]
    public IEnumerable<SnackItemViewModel>? SnackItems { get; }

    [ObservableAsProperty]
    public bool SnackItemsAvailable { get; }

    [Reactive]
    public SnackItemViewModel? SelectedSnackItem { get; set; }

    [ObservableAsProperty]
    public SnackEditViewModel? SelectedSnackEdit { get; }

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Unit> AddSnackCommand { get; }

    private void AddSnack()
    {
        if (!Initialized)
        {
            return;
        }
        var snack = new Snack { Name = "New Snack" };
        var snackItem = new SnackItemViewModel(snack);
        // var result = await Result.Ok()
        //                          .Ensure(_clusterClient != null, "Cluster client is not available")
        //                          .MapTry(() => _clusterClient!.GetGrain<ISnackRepoGrain>(string.Empty))
        //                          .BindTryAsync(grain => grain.CreateAsync(new SnackRepoCreateCommand()))
    }

    public ReactiveCommand<Unit, Unit> RemoveSnackCommand { get; }

    private void RemoveSnack()
    {
        if (!Initialized)
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
