using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;
using Orleans;
using Orleans.FluentResults;
using Orleans.Runtime;
using Orleans.Streams;
using ReactiveUI;
using SiloX.Domain.Abstractions;
using Vending.App.Snacks;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;
using Vending.Projection.Abstractions.Machines;
using Vending.Projection.Abstractions.Snacks;

namespace Vending.App.Machines;

public class MachinesManagementViewModel : ReactiveObject, IActivatableViewModel, IOrleansObject
{
    private readonly SourceCache<MachineViewModel, Guid> _machinesCache;
    private readonly SourceCache<SnackViewModel, Guid> _snacksCache;

    private StreamSubscriptionHandle<MachineInfoEvent>? _subscription;
    private StreamSequenceToken? _lastSequenceToken;

    private StreamSubscriptionHandle<SnackInfoEvent>? _snackSubscription;
    private StreamSequenceToken? _lastSnackSequenceToken;

    /// <inheritdoc />
    public MachinesManagementViewModel()
    {
        // Create the cache for the snacks.
        _snacksCache = new SourceCache<SnackViewModel, Guid>(snack => snack.Id);
        _snacksCache.Connect()
                    .Sort(SortExpressionComparer<SnackViewModel>.Ascending(snack => snack.Id))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out _snacks)
                    .Subscribe();
        // Recalculate the snack count when the snacks change.
        _snacksCache.CountChanged.ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(count => SnackCount = count);

        // Get snacks and update the cache.
        this.WhenAnyValue(vm => vm.ClusterClient)
            .Where(client => client != null)
            .Select(client => client!.GetGrain<ISnackRetrieverGrain>("Manager"))
            .SelectMany(grain => grain.ListAsync(new SnackRetrieverListQuery(null, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
            .Where(result => result.IsSuccess)
            .Select(result => result.Value)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(snacks => _snacksCache.Edit(updater => updater.AddOrUpdate(snacks.Select(snack => new SnackViewModel(snack)))));

        // Create the cache for the machines.
        _machinesCache = new SourceCache<MachineViewModel, Guid>(machine => machine.Id);
        // Connect the cache to the machines observable collection.
        var machinesObs = _machinesCache.Connect()
                                     .Filter(this.WhenAnyValue(vm => vm.MoneyAmountStart, vm => vm.MoneyAmountEnd)
                                                 .Throttle(TimeSpan.FromMilliseconds(500))
                                                 .DistinctUntilChanged()
                                                 .Select(tuple => new Func<MachineViewModel, bool>(machine => (tuple.Item1 == null || machine.MoneyInside.Amount >= tuple.Item1)
                                                                                                           && (tuple.Item2 == null || machine.MoneyInside.Amount < tuple.Item2)
                                                                                                           && machine.IsDeleted == false)));
        machinesObs.Sort(SortExpressionComparer<MachineViewModel>.Ascending(machine => machine.Id))
                .Page(this.WhenAnyValue(vm => vm.PageNumber, vm => vm.PageSize)
                          .DistinctUntilChanged()
                          .Select(tuple => new PageRequest(tuple.Item1, tuple.Item2)))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _machines)
                .Subscribe();
        // Recalculate the page count when the cache changes.
        machinesObs.Count()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(count =>
                           {
                               MachineCount = count;
                               PageCount = (int)Math.Ceiling((double)count / PageSize);
                           });
        this.WhenAnyValue(vm => vm.PageSize)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(size => PageCount = (int)Math.Ceiling((double)_machinesCache.Count / size));

        // Recalculate the page number when the page size or page count changes.
        this.WhenAnyValue(vm => vm.PageSize, vm => vm.PageCount)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(tuple =>
                       {
                           var pageNumber = PageNumber <= 0 ? 1 : PageNumber;
                           var oldOffset = _oldPageSize * (pageNumber - 1) + 1;
                           var newPageNumber = (int)Math.Ceiling((double)oldOffset / tuple.Item1);
                           pageNumber = Math.Min(newPageNumber, tuple.Item2);
                           PageNumber = pageNumber <= 0 ? 1 : pageNumber;
                       });

        // Get machines and update the cache.
        this.WhenAnyValue(vm => vm.MoneyAmountStart, vm => vm.MoneyAmountEnd, vm => vm.ClusterClient)
            .Where(tuple => tuple.Item3 != null)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .DistinctUntilChanged()
            .Select(tuple => (tuple.Item1, tuple.Item2, tuple.Item3!.GetGrain<IMachineRetrieverGrain>("Manager")))
            .SelectMany(tuple => tuple.Item3.ListAsync(new MachineRetrieverListQuery(null, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager", new DecimalRange(tuple.Item1, tuple.Item2))))
            .Where(result => result.IsSuccess)
            .Select(result => result.Value)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(machines => _machinesCache.Edit(updater => updater.AddOrUpdate(machines.Select(machine => new MachineViewModel(machine)))));

        // When the current machine changes, if it is null, set the current machine edit view model to null.
        this.WhenAnyValue(vm => vm.CurrentMachine, vm => vm.SnackCount, vm => vm.ClusterClient)
            .Where(tuple => tuple.Item1 == null || tuple.Item2 == 0 || tuple.Item3 == null)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => CurrentMachineEdit = null);

        // Stream events subscription.
        this.WhenActivated(disposable =>
                           {
                               // When the cluster client changes, subscribe to the machine info stream.
                               this.WhenAnyValue(vm => vm.ClusterClient)
                                   .Where(client => client != null)
                                   .Select(client => client!.GetStreamProvider(Constants.StreamProviderName))
                                   .Select(streamProvider => streamProvider.GetStream<MachineInfoEvent>(StreamId.Create(Constants.MachineInfosBroadcastNamespace, Guid.Empty)))
                                   .SelectMany(stream => stream.SubscribeAsync(HandleEventAsync, HandleErrorAsync, HandleCompletedAsync, _lastSequenceToken))
                                   .Subscribe(HandleSubscriptionAsync)
                                   .DisposeWith(disposable);
                               Disposable.Create(HandleSubscriptionDisposeAsync)
                                         .DisposeWith(disposable);

                               // When the cluster client changes, subscribe to the snack info stream.
                               this.WhenAnyValue(vm => vm.ClusterClient)
                                   .Where(client => client != null)
                                   .Select(client => client!.GetStreamProvider(Constants.StreamProviderName))
                                   .Select(streamProvider => streamProvider.GetStream<SnackInfoEvent>(StreamId.Create(Constants.SnackInfosBroadcastNamespace, Guid.Empty)))
                                   .SelectMany(stream => stream.SubscribeAsync(HandleSnackEventAsync, HandleSnackErrorAsync, HandleSnackCompletedAsync, _lastSnackSequenceToken))
                                   .Subscribe(HandleSnackSubscriptionAsync)
                                   .DisposeWith(disposable);
                               Disposable.Create(HandleSnackSubscriptionDisposeAsync)
                                         .DisposeWith(disposable);
                           });

        // Create the commands.
        AddMachineCommand = ReactiveCommand.CreateFromTask(AddMachineAsync, CanAddMachine);
        EditMachineCommand = ReactiveCommand.CreateFromTask(EditMachineAsync, CanEditMachine);
        RemoveMachineCommand = ReactiveCommand.CreateFromTask(RemoveMachineAsync, CanRemoveMachine);
        SyncMachinesCommand = ReactiveCommand.CreateFromTask(SyncMachinesAsync, CanSyncMachines);
        GoPreviousPageCommand = ReactiveCommand.Create(GoPreviousPage, CanGoPreviousPage);
        GoNextPageCommand = ReactiveCommand.Create(GoNextPage, CanGoNextPage);
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

    private int _pageSize = 10;
    private int _oldPageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set
        {
            _oldPageSize = _pageSize;
            this.RaiseAndSetIfChanged(ref _pageSize, value < 1 ? 1 : value);
        }
    }

    private int _pageCount;
    public int PageCount
    {
        get => _pageCount;
        set => this.RaiseAndSetIfChanged(ref _pageCount, value < 1 ? 1 : value);
    }

    private int _pageNumber = 1;
    public int PageNumber
    {
        get => _pageNumber;
        set => this.RaiseAndSetIfChanged(ref _pageNumber, value < 1 ? 1 : value);
    }

    private decimal? _moneyAmountStart;
    public decimal? MoneyAmountStart
    {
        get => _moneyAmountStart;
        set => this.RaiseAndSetIfChanged(ref _moneyAmountStart, value);
    }

    private decimal? _moneyAmountEnd;
    public decimal? MoneyAmountEnd
    {
        get => _moneyAmountEnd;
        set => this.RaiseAndSetIfChanged(ref _moneyAmountEnd, value);
    }

    private MachineViewModel? _currentMachine;
    public MachineViewModel? CurrentMachine
    {
        get => _currentMachine;
        set => this.RaiseAndSetIfChanged(ref _currentMachine, value);
    }

    private MachineEditWindowModel? _currentMachineEdit;
    public MachineEditWindowModel? CurrentMachineEdit
    {
        get => _currentMachineEdit;
        set => this.RaiseAndSetIfChanged(ref _currentMachineEdit, value);
    }

    private readonly ReadOnlyObservableCollection<MachineViewModel> _machines;
    public ReadOnlyObservableCollection<MachineViewModel> Machines => _machines;

    private int _machineCount;
    public int MachineCount
    {
        get => _machineCount;
        set => this.RaiseAndSetIfChanged(ref _machineCount, value);
    }

    private readonly ReadOnlyObservableCollection<SnackViewModel> _snacks;
    public ReadOnlyObservableCollection<SnackViewModel> Snacks => _snacks;

    private int _snackCount;
    public int SnackCount
    {
        get => _snackCount;
        set => this.RaiseAndSetIfChanged(ref _snackCount, value);
    }

    #endregion

    #region Commands

    /// <summary>
    ///     Gets the command that adds a new machine.
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddMachineCommand { get; }

    private IObservable<bool> CanAddMachine =>
        this.WhenAnyValue(vm => vm.SnackCount, vm => vm.ClusterClient)
            .Select(tuple => tuple is { Item1: > 0, Item2: not null });

    /// <summary>
    ///     Gets the interaction that shows the machine edit dialog.
    /// </summary>
    public Interaction<MachineEditWindowModel, Unit> ShowEditMachine { get; } = new();

    /// <summary>
    ///     Adds a new machine.
    /// </summary>
    private async Task AddMachineAsync()
    {
        bool retry;
        do
        {
            var result = Result.Ok()
                               .Ensure(SnackCount > 0, "No snacks available.")
                               .Ensure(ClusterClient != null, "No cluster client available.")
                               .MapTry(() =>
                                       {
                                           var machine = new Machine();
                                           machine.Slots.Add(new MachineSlot(machine.Id, 1));
                                           return new MachineEditWindowModel(machine, _snacks, ClusterClient!);
                                       });
            if (result.IsSuccess)
            {
                await ShowEditMachine.Handle(result.Value);
                return;
            }
            var errorRecovery = await Interactions.Errors.Handle(result.Errors);
            retry = errorRecovery == ErrorRecoveryOption.Retry;
        }
        while (retry);
    }

    /// <summary>
    ///     Gets the command that edits a machine.
    /// </summary>
    public ReactiveCommand<Unit, Unit> EditMachineCommand { get; }

    private IObservable<bool> CanEditMachine =>
        this.WhenAnyValue(vm => vm.CurrentMachine, vm => vm.SnackCount, vm => vm.ClusterClient)
            .Select(tuple => tuple is { Item1: not null, Item2: > 0, Item3: not null });

    /// <summary>
    ///     Edits the current machine.
    /// </summary>
    private async Task EditMachineAsync()
    {
        bool retry;
        do
        {
            var result = await Result.Ok()
                                     .Ensure(CurrentMachine != null, "No machine selected.")
                                     .Ensure(SnackCount > 0, "No snacks available.")
                                     .Ensure(ClusterClient != null, "No cluster client available.")
                                     .MapTry(() => ClusterClient!.GetGrain<IMachineGrain>(CurrentMachine!.Id))
                                     .MapTryAsync(grain => grain.GetMachineAsync())
                                     .TapTryAsync(machine => CurrentMachineEdit = new MachineEditWindowModel(machine, _snacks, ClusterClient!));
            if (result.IsSuccess)
            {
                await ShowEditMachine.Handle(CurrentMachineEdit!);
                return;
            }
            var errorRecovery = await Interactions.Errors.Handle(result.Errors);
            retry = errorRecovery == ErrorRecoveryOption.Retry;
        }
        while (retry);
    }

    /// <summary>
    ///     Gets the command that removes the current machine.
    /// </summary>
    public ReactiveCommand<Unit, Unit> RemoveMachineCommand { get; }

    /// <summary>
    ///     Gets the observable that indicates whether the remove machine command can be executed.
    /// </summary>
    private IObservable<bool> CanRemoveMachine =>
        this.WhenAnyValue(vm => vm.CurrentMachine, vm => vm.ClusterClient)
            .Select(tuple => tuple is { Item1: not null, Item2: not null });

    /// <summary>
    ///     Gets the interaction that asks the user to confirm the removal of the current machine.
    /// </summary>
    public Interaction<string, bool> ConfirmRemoveMachine { get; } = new();

    /// <summary>
    ///     Removes the current machine.
    /// </summary>
    private async Task RemoveMachineAsync()
    {
        var confirm = await ConfirmRemoveMachine.Handle(CurrentMachine!.Id.ToString());
        if (!confirm)
        {
            return;
        }
        bool retry;
        do
        {
            var result = await Result.Ok()
                                     .Ensure(CurrentMachine != null, "No machine selected.")
                                     .Ensure(ClusterClient != null, "No cluster client available.")
                                     .MapTry(() => ClusterClient!.GetGrain<IMachineRepoGrain>(string.Empty))
                                     .BindTryAsync(grain => grain.DeleteAsync(new MachineRepoDeleteCommand(CurrentMachine!.Id, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")));
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
    ///     Gets the command that syncs the machine data.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SyncMachinesCommand { get; }

    /// <summary>
    ///     Gets the observable that indicates whether the sync machines command can be executed.
    /// </summary>
    private IObservable<bool> CanSyncMachines =>
        this.WhenAnyValue(vm => vm.ClusterClient)
            .Select(client => client != null);

    /// <summary>
    ///     Syncs the machine data.
    /// </summary>
    private async Task SyncMachinesAsync()
    {
        bool retry;
        do
        {
            var result = await Result.Ok()
                                     .Ensure(ClusterClient != null, "No cluster client available.")
                                     .MapTry(() => ClusterClient!.GetGrain<IMachineSynchronizerGrain>(string.Empty))
                                     .MapTryAsync(grain => grain.SyncDifferencesAsync());
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
    ///     Gets the command that moves to the previous page.
    /// </summary>
    public ReactiveCommand<Unit, Unit> GoPreviousPageCommand { get; }

    private IObservable<bool> CanGoPreviousPage =>
        this.WhenAnyValue(vm => vm.PageNumber)
            .Select(number => number > 1);

    /// <summary>
    ///     Moves to the previous page.
    /// </summary>
    private void GoPreviousPage()
    {
        PageNumber--;
    }

    /// <summary>
    ///     Gets the command that moves to the next page.
    /// </summary>
    public ReactiveCommand<Unit, Unit> GoNextPageCommand { get; }

    private IObservable<bool> CanGoNextPage =>
        this.WhenAnyValue(vm => vm.PageNumber, vm => vm.PageCount)
            .Select(tuple => tuple.Item1 < tuple.Item2);

    /// <summary>
    ///     Moves to the next page.
    /// </summary>
    private void GoNextPage()
    {
        PageNumber++;
    }

    #endregion

    #region Stream Handlers

    private async void HandleSubscriptionAsync(StreamSubscriptionHandle<MachineInfoEvent> subscription)
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

    private Task HandleEventAsync(MachineInfoEvent projectionEvent, StreamSequenceToken sequenceToken)
    {
        _lastSequenceToken = sequenceToken;
        return projectionEvent switch
               {
                   MachineInfoSavedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineInfoErrorEvent machineEvent => ApplyErrorEventAsync(machineEvent),
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

    private Task ApplyEventAsync(MachineInfoSavedEvent machineEvent)
    {
        _machinesCache.Edit(updater => updater.AddOrUpdate(new MachineViewModel(machineEvent.Machine)));
        return Task.CompletedTask;
    }

    private Task ApplyErrorEventAsync(MachineInfoErrorEvent errorEvent)
    {
        return Task.CompletedTask;
    }

    #endregion

    #region Snack Stream Handlers

    private async void HandleSnackSubscriptionAsync(StreamSubscriptionHandle<SnackInfoEvent> snackSubscription)
    {
        if (_snackSubscription != null)
        {
            try
            {
                await _snackSubscription.UnsubscribeAsync();
            }
            catch
            {
                // ignored
            }
        }
        _snackSubscription = snackSubscription;
    }

    private async void HandleSnackSubscriptionDisposeAsync()
    {
        if (_snackSubscription == null)
        {
            return;
        }
        try
        {
            await _snackSubscription.UnsubscribeAsync();
        }
        catch
        {
            // ignored
        }
    }

    private Task HandleSnackEventAsync(SnackInfoEvent projectionEvent, StreamSequenceToken sequenceToken)
    {
        _lastSnackSequenceToken = sequenceToken;
        return projectionEvent switch
               {
                   SnackInfoSavedEvent snackEvent => ApplySnackEventAsync(snackEvent),
                   SnackInfoErrorEvent snackEvent => ApplySnackErrorEventAsync(snackEvent),
                   _ => Task.CompletedTask
               };
    }

    private Task HandleSnackErrorAsync(Exception exception)
    {
        return Task.CompletedTask;
    }

    private Task HandleSnackCompletedAsync()
    {
        return Task.CompletedTask;
    }

    private Task ApplySnackEventAsync(SnackInfoSavedEvent snackEvent)
    {
        _snacksCache.Edit(updater => updater.AddOrUpdate(new SnackViewModel(snackEvent.Snack)));
        return Task.CompletedTask;
    }

    private Task ApplySnackErrorEventAsync(SnackInfoErrorEvent errorEvent)
    {
        // return Interactions.Errors.Handle(result.Errors);
        return Task.CompletedTask;
    }

    #endregion

}