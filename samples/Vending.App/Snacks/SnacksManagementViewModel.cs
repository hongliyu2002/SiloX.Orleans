using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using DynamicData;
using DynamicData.Binding;
using Fluxera.Utilities.Extensions;
using Orleans;
using Orleans.FluentResults;
using Orleans.Runtime;
using Orleans.Streams;
using ReactiveUI;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Snacks;
using Vending.Projection.Abstractions.Snacks;

namespace Vending.App.Snacks;

public class SnacksManagementViewModel : ReactiveObject, IActivatableViewModel, IOrleansObject
{
    private readonly SourceCache<SnackItemViewModel, Guid> _snackItemsCache;
    private readonly ReadOnlyObservableCollection<SnackItemViewModel>? _snackItems;
    private StreamSubscriptionHandle<SnackInfoEvent>? _subscription;
    private StreamSequenceToken? _lastSequenceToken;

    /// <inheritdoc />
    public SnacksManagementViewModel()
    {
        // Create the cache for the snack items.
        _snackItemsCache = new SourceCache<SnackItemViewModel, Guid>(snack => snack.Id);
        // Connect to the cache and bind to the snack items.
        var searchTermObs = this.WhenAnyValue(vm => vm.SearchTerm)
                                .Throttle(TimeSpan.FromMilliseconds(500))
                                .DistinctUntilChanged()
                                .Select(term => new Func<SnackItemViewModel, bool>(item => (term.IsNullOrEmpty() || item.Name.Contains(term, StringComparison.OrdinalIgnoreCase)) && item.IsDeleted == false));
        _snackItemsCache.Connect()
                        .Filter(searchTermObs)
                        .Sort(SortExpressionComparer<SnackItemViewModel>.Ascending(item => item.Name))
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Bind(out _snackItems)
                        .Subscribe();
        // Search snack items and update the cache.
        this.WhenAnyValue(vm => vm.SearchTerm, vm => vm.ClusterClient)
            .Where(tc => tc.Item2 != null)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .DistinctUntilChanged()
            .Select(termClient => (termClient.Item1.Trim(), termClient.Item2!.GetGrain<ISnackRetrieverGrain>("Manager")))
            .SelectMany(termGrain => termGrain.Item2.SearchingListAsync(new SnackRetrieverSearchingListQuery(termGrain.Item1, new Dictionary<string, bool> { { "Id", false } }, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
            .Where(result => result.IsSuccess)
            .Select(result => result.Value)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(snacks => _snackItemsCache.Edit(updater => updater.AddOrUpdate(snacks.Select(snack => new SnackItemViewModel(snack)))));
        // When the current snack item changes, get the snack edit view model.
        this.WhenAnyValue(vm => vm.CurrentSnackItem, vm => vm.ClusterClient)
            .Where(itemClient => itemClient is { Item1: not null, Item2: not null })
            .Select(itemClient => itemClient.Item2!.GetGrain<ISnackGrain>(itemClient.Item1!.Id))
            .SelectMany(grain => grain.GetSnackAsync())
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(snack => CurrentSnackEdit = new SnackEditViewModel(snack, ClusterClient!));
        this.WhenActivated(disposable =>
                           {
                               // When the cluster client changes, subscribe to the snack info stream.
                               this.WhenAnyValue(vm => vm.ClusterClient)
                                   .Where(client => client != null)
                                   .Select(client => client!.GetStreamProvider(Constants.StreamProviderName))
                                   .Select(provider => provider.GetStream<SnackInfoEvent>(StreamId.Create(Constants.SnackInfosBroadcastNamespace, Guid.Empty)))
                                   .SelectMany(stream => stream.SubscribeAsync(HandleEventAsync, HandleErrorAsync, HandleCompletedAsync, _lastSequenceToken))
                                   .ObserveOn(RxApp.MainThreadScheduler)
                                   .Subscribe(HandleSubscriptionAsync)
                                   .DisposeWith(disposable);
                               Disposable.Create(HandleSubscriptionDisposeAsync)
                                         .DisposeWith(disposable);
                           });
        // Create the commands.
        AddSnackCommand = ReactiveCommand.CreateFromTask(AddSnackAsync, CanAddSnack);
        RemoveSnackCommand = ReactiveCommand.CreateFromTask(RemoveSnackAsync, CanRemoveSnack);
        MoveNavigationSideCommand = ReactiveCommand.Create(MoveNavigationSide);
    }

    #region Stream Handlers

    private async void HandleSubscriptionAsync(StreamSubscriptionHandle<SnackInfoEvent> subscription)
    {
        if (_subscription != null)
        {
            try
            {
                await _subscription.UnsubscribeAsync();
            }
            catch
            {
                // ignored
            }
        }
        _subscription = subscription;
    }

    private async void HandleSubscriptionDisposeAsync()
    {
        if (_subscription == null)
        {
            return;
        }
        try
        {
            await _subscription.UnsubscribeAsync();
        }
        catch
        {
            // ignored
        }
    }

    private Task HandleEventAsync(SnackInfoEvent projectionEvent, StreamSequenceToken sequenceToken)
    {
        _lastSequenceToken = sequenceToken;
        switch (projectionEvent)
        {
            case SnackInfoSavedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case SnackInfoErrorEvent snackEvent:
                return ApplyErrorEventAsync(snackEvent);
            default:
                return Task.CompletedTask;
        }
    }

    private Task HandleErrorAsync(Exception exception)
    {
        return Task.CompletedTask;
    }

    private Task HandleCompletedAsync()
    {
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(SnackInfoSavedEvent snackEvent)
    {
        _snackItemsCache.Edit(updater => updater.AddOrUpdate(new SnackItemViewModel(snackEvent.Snack)));
        return Task.CompletedTask;
    }

    private Task ApplyErrorEventAsync(SnackInfoErrorEvent errorEvent)
    {
        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    public ViewModelActivator Activator { get; } = new();

    private IClusterClient? _clusterClient;

    public IClusterClient? ClusterClient
    {
        get => _clusterClient;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _clusterClient, value);
                               });
        }
    }

    private NavigationSide _navigationSide;

    public NavigationSide NavigationSide
    {
        get => _navigationSide;
        set => this.RaiseAndSetIfChanged(ref _navigationSide, value);
    }

    private string _searchTerm = string.Empty;

    public string SearchTerm
    {
        get => _searchTerm;
        set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
    }

    private SnackItemViewModel? _currentSnackItem;

    public SnackItemViewModel? CurrentSnackItem
    {
        get => _currentSnackItem;
        set => this.RaiseAndSetIfChanged(ref _currentSnackItem, value);
    }

    private SnackEditViewModel? _currentSnackEdit;

    public SnackEditViewModel? CurrentSnackEdit
    {
        get => _currentSnackEdit;
        set => this.RaiseAndSetIfChanged(ref _currentSnackEdit, value);
    }

    public ReadOnlyObservableCollection<SnackItemViewModel>? SnackItems => _snackItems;

    #endregion

    #region Commands

    /// <summary>
    ///     Gets the command that adds a new snack.
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddSnackCommand { get; }

    /// <summary>
    ///     Gets the observable that determines whether the add snack command can be executed.
    /// </summary>
    private IObservable<bool> CanAddSnack =>
        this.WhenAnyValue(vm => vm.ClusterClient)
            .Select(client => client != null);

    /// <summary>
    ///     Adds a new snack.
    /// </summary>
    private async Task AddSnackAsync()
    {
        bool retry;
        do
        {
            var result = Result.Ok()
                               .Map(() => CurrentSnackEdit = new SnackEditViewModel(new Snack(), ClusterClient!));
            if (result.IsSuccess)
            {
                return;
            }
            var errorRecovery = await Interactions.Errors.Handle(result.Errors);
            retry = errorRecovery == ErrorRecoveryOption.Retry;
        }
        while (retry);
    }

    /// <summary>
    ///     Gets the command that removes the current snack.
    /// </summary>
    public ReactiveCommand<Unit, Unit> RemoveSnackCommand { get; }

    /// <summary>
    ///     Gets the observable that indicates whether the remove snack command can be executed.
    /// </summary>
    private IObservable<bool> CanRemoveSnack =>
        this.WhenAnyValue(vm => vm.CurrentSnackItem, vm => vm.ClusterClient)
            .Select(itemClient => itemClient is { Item1: not null, Item2: not null });

    /// <summary>
    ///     Gets the interaction that asks the user to confirm the removal of the current snack.
    /// </summary>
    public Interaction<string, bool> ConfirmRemoveSnack { get; } = new();

    /// <summary>
    ///     Removes the current snack.
    /// </summary>
    private async Task RemoveSnackAsync()
    {
        var confirm = await ConfirmRemoveSnack.Handle(CurrentSnackItem!.Name);
        if (!confirm)
        {
            return;
        }
        bool retry;
        do
        {
            var result = await Result.Ok()
                                     .MapTry(() => ClusterClient!.GetGrain<ISnackRepoGrain>(string.Empty))
                                     .BindTryAsync(repoGrain => repoGrain.DeleteAsync(new SnackRepoDeleteCommand(CurrentSnackItem!.Id, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")));
            if (result.IsSuccess)
            {
                return;
            }
            var errorRecovery = await Interactions.Errors.Handle(result.Errors);
            retry = errorRecovery == ErrorRecoveryOption.Retry;
        }
        while (retry);
    }

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