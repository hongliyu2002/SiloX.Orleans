using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using Orleans;
using Orleans.FluentResults;
using Orleans.Runtime;
using Orleans.Streams;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Snacks;
using Vending.Projection.Abstractions.Snacks;

namespace Vending.App.Snacks;

public class SnacksManagementViewModel : ReactiveObject, IActivatableViewModel
{
    private readonly SourceCache<SnackItemViewModel, Guid> _snackItemsCache;
    private ReadOnlyObservableCollection<SnackItemViewModel>? _snackItems;

    private StreamSubscriptionHandle<SnackInfoSavedEvent>? _subscription;

    /// <inheritdoc />
    public SnacksManagementViewModel(MainWindowModel mainModel)
    {
        Activator = new ViewModelActivator();
        Guard.Against.Null(mainModel, nameof(mainModel));
        // Create the cache for the snack items.
        _snackItemsCache = new SourceCache<SnackItemViewModel, Guid>(snack => snack.Id);
        this.WhenActivated(disposable =>
                           {
                               // Get the cluster client from the main view model.
                               mainModel.WhenAnyValue(vm => vm.ClusterClient)
                                        .Where(client => client != null)
                                        .ToPropertyEx(this, vm => vm.ClusterClient)
                                        .DisposeWith(disposable);
                               _snackItemsCache.DisposeWith(disposable);
                               // Connect to the cache and bind to the snack items.
                               _snackItemsCache.Connect()
                                               .Filter(snack => SearchTerm.IsNullOrEmpty() || snack.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                                               .ObserveOn(RxApp.MainThreadScheduler)
                                               .Bind(out _snackItems)
                                               .Subscribe()
                                               .DisposeWith(disposable);
                               Disposable.Create(() => _subscription?.UnsubscribeAsync()
                                                                     .Wait())
                                         .DisposeWith(disposable);
                           });
        // Subscribe to snack info saved events.
        this.WhenAnyValue(vm => vm.ClusterClient)
            .Where(client => client != null)
            .Select(client => client!.GetStreamProvider(Constants.StreamProviderName))
            .Select(streamProvider => streamProvider.GetStream<SnackInfoSavedEvent>(StreamId.Create(Constants.SnackInfosBroadcastNamespace, string.Empty)))
            .SelectMany(stream => stream.SubscribeAsync(HandleEvents, HandleError, HandleCompleted))
            .SubscribeOn(RxApp.MainThreadScheduler)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(handle => _subscription = handle);
        // Search snack items and update the cache.
        this.WhenAnyValue(vm => vm.SearchTerm, vm => vm.ClusterClient)
            .Where(termAndClient => termAndClient.Item2 != null)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .DistinctUntilChanged()
            .Select(termAndClient => (termAndClient.Item1.Trim(), termAndClient.Item2!.GetGrain<ISnackRetrieverGrain>("Manager")))
            .SelectMany(termAndGrain => termAndGrain.Item2.SearchingListAsync(new SnackRetrieverSearchingListQuery(termAndGrain.Item1, new Dictionary<string, bool> { { "CreatedAt", true } }, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
            .Where(result => result.IsSuccess)
            .Select(result => result.Value)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(snacks => _snackItemsCache.Edit(updater => updater.AddOrUpdate(snacks.Select(snack => new SnackItemViewModel(snack)))));
        // When the current snack item changes, get the snack edit view model.
        this.WhenAnyValue(vm => vm.CurrentSnackItem, vm => vm.ClusterClient)
            .Where(itemAndClient => itemAndClient is { Item1: not null, Item2: not null })
            .Select(itemAndClient => itemAndClient.Item2!.GetGrain<ISnackGrain>(itemAndClient.Item1!.Id))
            .SelectMany(snackGrain => snackGrain.GetSnackAsync())
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(snack => CurrentSnackEdit = new SnackEditViewModel(snack, ClusterClient!.GetGrain<ISnackRepoGrain>("Manager")));
        AddSnackCommand = ReactiveCommand.Create(AddSnackAsync);
        RemoveSnackCommand = ReactiveCommand.CreateFromTask(RemoveSnackAsync, CanRemoveSnack);
        MoveNavigationSideCommand = ReactiveCommand.Create(MoveNavigationSide);
    }

    #region Stream Handlers

    private Task HandleEvents(IList<SequentialItem<SnackInfoSavedEvent>> events)
    {
        foreach (var evt in events.Select(evtItem => evtItem.Item))
        {
            _snackItemsCache.Edit(updater => updater.AddOrUpdate(new SnackItemViewModel(evt.Snack)));
        }
        return Task.CompletedTask;
    }

    private Task HandleError(Exception exception)
    {
        return Task.CompletedTask;
    }

    private Task HandleCompleted()
    {
        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    public ViewModelActivator Activator { get; }

    [ObservableAsProperty]
    public IClusterClient? ClusterClient { get; }

    [Reactive]
    public NavigationSide NavigationSide { get; set; }

    [Reactive]
    public string SearchTerm { get; set; } = string.Empty;

    public ReadOnlyObservableCollection<SnackItemViewModel>? SnackItems => _snackItems;

    [Reactive]
    public SnackItemViewModel? CurrentSnackItem { get; set; }

    [Reactive]
    public SnackEditViewModel? CurrentSnackEdit { get; set; }

    #endregion

    #region Commands

    /// <summary>
    ///     Gets the command that adds a new snack.
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddSnackCommand { get; }

    /// <summary>
    ///     Gets the command that moves the navigation side.
    /// </summary>
    private void AddSnackAsync()
    {
        ISnackRepoGrain repoGrain = null!;
        Result.Ok()
              .Ensure(ClusterClient != null, "Cluster client is not available.")
              .MapTry(() => repoGrain = ClusterClient!.GetGrain<ISnackRepoGrain>(string.Empty))
              .Map(() => CurrentSnackEdit = new SnackEditViewModel(new Snack(), repoGrain));
    }

    /// <summary>
    ///     Gets the command that removes the current snack.
    /// </summary>
    public ReactiveCommand<Unit, Unit> RemoveSnackCommand { get; }

    /// <summary>
    ///     Removes the current snack.
    /// </summary>
    private async Task RemoveSnackAsync()
    {
        ISnackRepoGrain repoGrain = null!;
        await Result.Ok()
                    .Ensure(ClusterClient != null, "Cluster client is not available.")
                    .MapTry(() => repoGrain = ClusterClient!.GetGrain<ISnackRepoGrain>(string.Empty))
                    .Ensure(CurrentSnackItem != null, "Snack item should be selected.")
                    .BindTryAsync(() => repoGrain.DeleteAsync(new SnackRepoDeleteCommand(CurrentSnackItem!.Id, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")));
    }

    /// <summary>
    ///     Gets the observable that indicates whether the remove snack command can be executed.
    /// </summary>
    private IObservable<bool> CanRemoveSnack =>
        this.WhenAnyValue(vm => vm.CurrentSnackItem, vm => vm.ClusterClient)
            .Select(itemAndClient => itemAndClient is { Item1: not null, Item2: not null });

    /// <summary>
    ///     Gets the command that moves the navigation side.
    /// </summary>
    public ReactiveCommand<Unit, Unit> MoveNavigationSideCommand { get; }

    /// <summary>
    ///     Moves the navigation side.
    /// </summary>
    private void MoveNavigationSide()
    {
        NavigationSide = NavigationSide == NavigationSide.Left ? NavigationSide.Right : NavigationSide.Left;
    }

    #endregion

}