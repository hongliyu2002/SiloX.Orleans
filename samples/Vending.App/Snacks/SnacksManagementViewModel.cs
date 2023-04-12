using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
    private readonly SourceCache<SnackViewModel, Guid> _snacksCache;

    private StreamSubscriptionHandle<SnackInfoEvent>? _subscription;
    private StreamSequenceToken? _lastSequenceToken;

    /// <inheritdoc />
    public SnacksManagementViewModel()
    {
        // Create the cache for the snacks.
        _snacksCache = new SourceCache<SnackViewModel, Guid>(snack => snack.Id);
        _snacksCache.Connect()
                    .Filter(this.WhenAnyValue(vm => vm.SearchTerm)
                                .Throttle(TimeSpan.FromMilliseconds(500))
                                .DistinctUntilChanged()
                                .Select(_ => new Func<SnackViewModel, bool>(snack => (SearchTerm.IsNullOrEmpty() || snack.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)) && snack.IsDeleted == false)))
                    .Sort(SortExpressionComparer<SnackViewModel>.Ascending(snack => snack.Id))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out _snacks)
                    .Subscribe();

        // Search snacks and update the cache.
        this.WhenAnyValue(vm => vm.SearchTerm, vm => vm.ClusterClient)
            .Where(tuple => tuple.Item2 != null)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .DistinctUntilChanged()
            .Select(tuple => (tuple.Item1, tuple.Item2!.GetGrain<ISnackRetrieverGrain>("Manager")))
            .SelectMany(tuple => tuple.Item2.SearchingListAsync(new SnackRetrieverSearchingListQuery(tuple.Item1, null, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
            .Where(result => result.IsSuccess)
            .Select(result => result.Value)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(snacks => _snacksCache.Edit(updater => updater.AddOrUpdate(snacks.Select(snack => new SnackViewModel(snack)))));

        // When the current snack changes, get the snack edit view model.
        this.WhenAnyValue(vm => vm.CurrentSnack, vm => vm.ClusterClient)
            .Where(tuple => tuple is { Item1: not null, Item2: not null })
            .Select(tuple => tuple.Item2!.GetGrain<ISnackGrain>(tuple.Item1!.Id))
            .SelectMany(grain => grain.GetSnackAsync())
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(snack => CurrentSnackEdit = new SnackEditViewModel(snack, ClusterClient!));
        this.WhenActivated(disposable =>
                           {
                               // When the cluster client changes, subscribe to the snack info stream.
                               this.WhenAnyValue(vm => vm.ClusterClient)
                                   .Where(client => client != null)
                                   .Select(client => client!.GetStreamProvider(Constants.StreamProviderName))
                                   .Select(streamProvider => streamProvider.GetStream<SnackInfoEvent>(StreamId.Create(Constants.SnackInfosBroadcastNamespace, Guid.Empty)))
                                   .SelectMany(stream => stream.SubscribeAsync(HandleEventAsync, HandleErrorAsync, HandleCompletedAsync, _lastSequenceToken))
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

    #region Properties

    /// <inheritdoc />
    public ViewModelActivator Activator { get; } = new();

    private IClusterClient? _clusterClient;
    public IClusterClient? ClusterClient
    {
        get => _clusterClient;
        set => this.RaiseAndSetIfChanged(ref _clusterClient, value);
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

    private SnackViewModel? _currentSnack;
    public SnackViewModel? CurrentSnack
    {
        get => _currentSnack;
        set => this.RaiseAndSetIfChanged(ref _currentSnack, value);
    }

    private SnackEditViewModel? _currentSnackEdit;
    public SnackEditViewModel? CurrentSnackEdit
    {
        get => _currentSnackEdit;
        set => this.RaiseAndSetIfChanged(ref _currentSnackEdit, value);
    }

    private readonly ReadOnlyObservableCollection<SnackViewModel> _snacks;
    public ReadOnlyObservableCollection<SnackViewModel> Snacks => _snacks;

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
                               .Ensure(ClusterClient != null, "No cluster client available.")
                               .MapTry(() =>
                                       {
                                           var snack = new Snack();
                                           return new SnackEditViewModel(snack, ClusterClient!);
                                       });
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
        this.WhenAnyValue(vm => vm.CurrentSnack, vm => vm.ClusterClient)
            .Select(tuple => tuple is { Item1: not null, Item2: not null });

    /// <summary>
    ///     Gets the interaction that asks the user to confirm the removal of the current snack.
    /// </summary>
    public Interaction<string, bool> ConfirmRemoveSnack { get; } = new();

    /// <summary>
    ///     Removes the current snack.
    /// </summary>
    private async Task RemoveSnackAsync()
    {
        var confirm = await ConfirmRemoveSnack.Handle(CurrentSnack!.Name);
        if (!confirm)
        {
            return;
        }
        bool retry;
        do
        {
            var result = await Result.Ok()
                                     .Ensure(CurrentSnack != null, "No snack selected.")
                                     .Ensure(ClusterClient != null, "No cluster client available.")
                                     .MapTry(() => ClusterClient!.GetGrain<ISnackRepoGrain>(string.Empty))
                                     .BindTryAsync(grain => grain.DeleteAsync(new SnackRepoDeleteCommand(CurrentSnack!.Id, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")));
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
        return projectionEvent switch
               {
                   SnackInfoSavedEvent snackEvent => ApplyEventAsync(snackEvent),
                   SnackInfoErrorEvent snackEvent => ApplyErrorEventAsync(snackEvent),
                   _ => Task.CompletedTask
               };
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
        _snacksCache.Edit(updater => updater.AddOrUpdate(new SnackViewModel(snackEvent.Snack)));
        return Task.CompletedTask;
    }

    private Task ApplyErrorEventAsync(SnackInfoErrorEvent errorEvent)
    {
        // return Interactions.Errors.Handle(result.Errors);
        return Task.CompletedTask;
    }

    #endregion

}