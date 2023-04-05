﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Fluxera.Utilities.Extensions;
using Orleans;
using Orleans.FluentResults;
using ReactiveUI;
using Splat;
using Vending.Projection.Abstractions.Snacks;

namespace Vending.App.ViewModels;

public class SnacksManagementViewModel : ReactiveObject
{
    private readonly IClusterClient? _clusterClient;

    /// <inheritdoc />
    public SnacksManagementViewModel()
    {
        _clusterClient = Locator.Current.GetService<IClusterClient>();
        _snackItems = this.WhenAnyValue(vm => vm.SearchTerm)
                          .Throttle(TimeSpan.FromMilliseconds(500))
                          .Select(term => term?.Trim())
                          .DistinctUntilChanged()
                          .Where(term => !string.IsNullOrEmpty(term))
                          .SelectMany(term => GetSnackItems(term!))
                          .ObserveOn(RxApp.MainThreadScheduler)
                          .ToProperty(this, vm => vm.SnackItems);
        _isSnackItemsAvailable = this.WhenAnyValue(vm => vm.SnackItems).Select(items => items.IsNotNullOrEmpty()).ToProperty(this, vm => vm.IsSnackItemsAvailable);
        AddSnackCommand = ReactiveCommand.Create(AddSnack);
        RemoveSnackCommand = ReactiveCommand.Create(RemoveSnack);
        MoveNavigationSideCommand = ReactiveCommand.Create(MoveNavigationSide);
    }

    private async Task<IEnumerable<SnackItemViewModel>> GetSnackItems(string? term)
    {
        var sortings = new Dictionary<string, bool>
                       {
                           {
                               "CreatedAt", true
                           }
                       };
        var result = await Result.Ok()
                                 .Ensure(_clusterClient != null, "Cluster client is not available")
                                 .MapTry(() => _clusterClient!.GetGrain<ISnackRetrieverGrain>(string.Empty))
                                 .BindTryAsync(grain => grain.SearchingListAsync(new SnackRetrieverSearchingListQuery(term, null, null, null, null, null, null, null, null, null, false, sortings,
                                                                                                                      Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
                                 .MapAsync(snacks => snacks.Select(snack => new SnackItemViewModel(snack)));
        return result.IsSuccess ? result.Value : Enumerable.Empty<SnackItemViewModel>();
    }

    #region Properties

    private NavigationSide _navigationSide;
    public NavigationSide NavigationSide
    {
        get => _navigationSide;
        set => this.RaiseAndSetIfChanged(ref _navigationSide, value);
    }

    private string? _searchTerm;

    public string? SearchTerm
    {
        get => _searchTerm;
        set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
    }

    private readonly ObservableAsPropertyHelper<IEnumerable<SnackItemViewModel>> _snackItems;
    public IEnumerable<SnackItemViewModel> SnackItems => _snackItems.Value;

    private readonly ObservableAsPropertyHelper<bool> _isSnackItemsAvailable;
    public bool IsSnackItemsAvailable => _isSnackItemsAvailable.Value;

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Unit> AddSnackCommand { get; }

    private void AddSnack()
    {
    }

    public ReactiveCommand<Unit, Unit> RemoveSnackCommand { get; }

    private void RemoveSnack()
    {
    }

    public ReactiveCommand<Unit, Unit> MoveNavigationSideCommand { get; }

    private void MoveNavigationSide()
    {
        NavigationSide = NavigationSide == NavigationSide.Left ? NavigationSide.Right : NavigationSide.Left;
    }

    #endregion

}
