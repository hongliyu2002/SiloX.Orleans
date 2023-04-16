using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Fluxera.Utilities.Extensions;
using Orleans.FluentResults;
using Orleans.Streams;
using ReactiveUI;
using SiloX.Domain.Abstractions.Extensions;
using Splat;
using Vending.Apps.Wpf.Services;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Snacks;
using Vending.Projection.Abstractions.Snacks;

namespace Vending.Apps.Wpf.Snacks;

public class SnacksManagementViewModel : ReactiveObject, IActivatableViewModel
{
    private StreamSequenceToken? _snackSequenceToken;

    /// <inheritdoc />
    public SnacksManagementViewModel()
    {
        // When the cluster client is ready, set the cluster client.
        ClusterClientReady = Locator.Current.GetService<IClusterClientReady>();
        this.WhenAnyValue(vm => vm.ClusterClientReady)
            .Where(clientReady => clientReady != null)
            .SelectMany(clientReady => clientReady!.ClusterClient.Task)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(client => ClusterClient = client);

        // Create the cache for the snacks.
        var snacksCache = new SourceCache<SnackViewModel, Guid>(snack => snack.Id);
        snacksCache.Connect()
                   .AutoRefresh(snack => snack.Id)
                   .Filter(this.WhenAnyValue(vm => vm.SearchTerm)
                               .Throttle(TimeSpan.FromMilliseconds(500))
                               .DistinctUntilChanged()
                               .Select(_ => new Func<SnackViewModel, bool>(snack => (SearchTerm.IsNullOrEmpty() || snack.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)) && snack.IsDeleted == false)))
                   .Sort(SortExpressionComparer<SnackViewModel>.Ascending(snack => snack.Id))
                   .ObserveOn(RxApp.MainThreadScheduler)
                   .Bind(out var snacks)
                   .Subscribe();
        Snacks = snacks;

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
            .Subscribe(snacksList => snacksCache.AddOrUpdateWith(snacksList));

        // When the current snack changes, get the snack edit view model.
        this.WhenAnyValue(vm => vm.CurrentSnack, vm => vm.ClusterClient)
            .Where(tuple => tuple is { Item1: not null, Item2: not null })
            .Select(tuple => tuple.Item2!.GetGrain<ISnackGrain>(tuple.Item1!.Id))
            .SelectMany(grain => grain.GetSnackAsync())
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(snack => CurrentSnackEdit = new SnackEditViewModel(snack, ClusterClient!));

        // Subscribe to the snack info stream.
        this.WhenActivated(disposable =>
                           {
                               // When the cluster client changes, subscribe to the snack info stream.
                               var allSnacksStreamObs = this.WhenAnyValue(vm => vm.ClusterClient)
                                                            .Where(client => client != null)
                                                            .SelectMany(client => client!.GetReceiverStreamWithGuidKey<SnackInfoEvent>(Constants.StreamProviderName, Constants.SnackInfosBroadcastNamespace, _snackSequenceToken))
                                                            .Publish()
                                                            .RefCount();
                               allSnacksStreamObs.Where(tuple => tuple.Event is SnackInfoSavedEvent)
                                                 .ObserveOn(RxApp.MainThreadScheduler)
                                                 .Subscribe(tuple =>
                                                            {
                                                                _snackSequenceToken = tuple.SequenceToken;
                                                                var savedEvent = (SnackInfoSavedEvent)tuple.Event;
                                                                snacksCache.AddOrUpdateWith(savedEvent.Snack);
                                                            })
                                                 .DisposeWith(disposable);
                               allSnacksStreamObs.Where(tuple => tuple.Event is SnackInfoErrorEvent)
                                                 .ObserveOn(RxApp.MainThreadScheduler)
                                                 .Subscribe(tuple =>
                                                            {
                                                                var errorEvent = (SnackInfoErrorEvent)tuple.Event;
                                                                ErrorInfo = $"{errorEvent.Code}:{string.Join("\n", errorEvent.Reasons)}";
                                                            })
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

    private readonly IClusterClientReady? _clusterClientReady;
    public IClusterClientReady? ClusterClientReady
    {
        get => _clusterClientReady;
        init => this.RaiseAndSetIfChanged(ref _clusterClientReady, value);
    }

    private IClusterClient? _clusterClient;
    public IClusterClient? ClusterClient
    {
        get => _clusterClient;
        set => this.RaiseAndSetIfChanged(ref _clusterClient, value);
    }

    private string _errorInfo = string.Empty;
    public string ErrorInfo
    {
        get => _errorInfo;
        set => this.RaiseAndSetIfChanged(ref _errorInfo, value);
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

    public ReadOnlyObservableCollection<SnackViewModel> Snacks { get; }

    #endregion

    #region Interactions

    /// <summary>
    ///     Interaction that asks the user to confirm the removal of the current snack.
    /// </summary>
    public Interaction<string, bool> ConfirmRemoveSnackInteraction { get; } = new();

    /// <summary>
    ///     Interaction for errors.
    /// </summary>
    public Interaction<IEnumerable<IError>, ErrorRecovery> ErrorsInteraction { get; } = new();

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
            var errorRecovery = await ErrorsInteraction.Handle(result.Errors);
            retry = errorRecovery == ErrorRecovery.Retry;
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
    ///     Removes the current snack.
    /// </summary>
    private async Task RemoveSnackAsync()
    {
        var confirm = await ConfirmRemoveSnackInteraction.Handle(CurrentSnack!.Name);
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
            var errorRecovery = await ErrorsInteraction.Handle(result.Errors);
            retry = errorRecovery == ErrorRecovery.Retry;
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