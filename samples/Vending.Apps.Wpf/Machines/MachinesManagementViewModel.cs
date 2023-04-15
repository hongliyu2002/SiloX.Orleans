using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;
using Orleans.FluentResults;
using Orleans.Streams;
using ReactiveUI;
using SiloX.Domain.Abstractions;
using SiloX.Domain.Abstractions.Extensions;
using Vending.App.Wpf.Snacks;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;
using Vending.Projection.Abstractions.Machines;
using Vending.Projection.Abstractions.Snacks;

namespace Vending.App.Wpf.Machines;

public class MachinesManagementViewModel : ReactiveObject, IActivatableViewModel, IOrleansObject
{
    private StreamSequenceToken? _machineSequenceToken;
    private StreamSequenceToken? _snackSequenceToken;

    /// <inheritdoc />
    public MachinesManagementViewModel()
    {
        // Create the cache for the snacks.
        var snacksCache = new SourceCache<SnackViewModel, Guid>(snack => snack.Id);
        snacksCache.Connect()
                   .Sort(SortExpressionComparer<SnackViewModel>.Ascending(snack => snack.Id))
                   .ObserveOn(RxApp.MainThreadScheduler)
                   .Bind(out var snacks)
                   .Subscribe();
        Snacks = snacks;
        // Recalculate the snack count when the snacks change.
        snacksCache.CountChanged.ObserveOn(RxApp.MainThreadScheduler)
                   .Subscribe(count => SnackCount = count);

        // Get snacks and update the cache.
        this.WhenAnyValue(vm => vm.ClusterClient)
            .Where(client => client != null)
            .Select(client => client!.GetGrain<ISnackRetrieverGrain>("Manager"))
            .SelectMany(grain => grain.ListAsync(new SnackRetrieverListQuery(null, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
            .Where(result => result.IsSuccess)
            .Select(result => result.Value)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(snacksList => snacksCache.Edit(updater => updater.AddOrUpdate(snacksList.Select(snack => new SnackViewModel(snack)))));

        // Create the cache for the machines.
        var machinesCache = new SourceCache<MachineViewModel, Guid>(machine => machine.Id);
        var machinesObs = machinesCache.Connect()
                                       .Filter(this.WhenAnyValue(vm => vm.MoneyAmountStart, vm => vm.MoneyAmountEnd)
                                                   .Throttle(TimeSpan.FromMilliseconds(500))
                                                   .DistinctUntilChanged()
                                                   .Select(tuple => new Func<MachineViewModel, bool>(machine => (tuple.Item1 == null || machine.MoneyInside.Amount >= tuple.Item1)
                                                                                                             && (tuple.Item2 == null || machine.MoneyInside.Amount < tuple.Item2)
                                                                                                             && machine.IsDeleted == false)))
                                       .Publish()
                                       .RefCount();
        // Sort and page the machines.
        machinesObs.Sort(SortExpressionComparer<MachineViewModel>.Ascending(machine => machine.Id))
                   .Page(this.WhenAnyValue(vm => vm.PageNumber, vm => vm.PageSize)
                             .DistinctUntilChanged()
                             .Select(tuple => new PageRequest(tuple.Item1, tuple.Item2)))
                   .ObserveOn(RxApp.MainThreadScheduler)
                   .Bind(out var machines)
                   .Subscribe();
        Machines = machines;
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
            .Subscribe(size => PageCount = (int)Math.Ceiling((double)machinesCache.Count / size));

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
            .Subscribe(machinesList => machinesCache.Edit(updater => updater.AddOrUpdate(machinesList.Select(machine => new MachineViewModel(machine)))));

        // When the current machine changes, if it is null, set the current machine edit view model to null.
        this.WhenAnyValue(vm => vm.CurrentMachine, vm => vm.SnackCount, vm => vm.ClusterClient)
            .Where(tuple => tuple.Item1 == null || tuple.Item2 == 0 || tuple.Item3 == null)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => CurrentMachineEdit = null);

        // Stream events subscription.
        this.WhenActivated(disposable =>
                           {
                               // When the cluster client changes, subscribe to the machine info stream.
                               var allMachinesStreamObs = this.WhenAnyValue(vm => vm.ClusterClient)
                                                              .Where(client => client != null)
                                                              .SelectMany(client => client!.GetReceiverStreamWithGuidKey<MachineInfoEvent>(Constants.StreamProviderName, Constants.MachineInfosBroadcastNamespace, _machineSequenceToken))
                                                              .Publish()
                                                              .RefCount();
                               allMachinesStreamObs.Where(tuple => tuple.Event is MachineInfoSavedEvent)
                                                   .ObserveOn(RxApp.MainThreadScheduler)
                                                   .Subscribe(tuple =>
                                                              {
                                                                  _machineSequenceToken = tuple.SequenceToken;
                                                                  var savedEvent = (MachineInfoSavedEvent)tuple.Event;
                                                                  machinesCache.Edit(updater => updater.AddOrUpdate(new MachineViewModel(savedEvent.Machine)));
                                                              })
                                                   .DisposeWith(disposable);
                               allMachinesStreamObs.Where(tuple => tuple.Event is MachineInfoErrorEvent)
                                                   .ObserveOn(RxApp.MainThreadScheduler)
                                                   .Subscribe(tuple =>
                                                              {
                                                                  var errorEvent = (MachineInfoErrorEvent)tuple.Event;
                                                                  ErrorInfo = $"{errorEvent.Code}:{string.Join("\n", errorEvent.Reasons)}";
                                                              })
                                                   .DisposeWith(disposable);

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
                                                                snacksCache.Edit(updater => updater.AddOrUpdate(new SnackViewModel(savedEvent.Snack)));
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

    private string _errorInfo = string.Empty;
    public string ErrorInfo
    {
        get => _errorInfo;
        set => this.RaiseAndSetIfChanged(ref _errorInfo, value);
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

    public ReadOnlyObservableCollection<MachineViewModel> Machines { get; }

    private int _machineCount;
    public int MachineCount
    {
        get => _machineCount;
        set => this.RaiseAndSetIfChanged(ref _machineCount, value);
    }

    public ReadOnlyObservableCollection<SnackViewModel> Snacks { get; }

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
                                           return new MachineEditWindowModel(machine, Snacks, ClusterClient!);
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
                                     .TapTryAsync(machine => CurrentMachineEdit = new MachineEditWindowModel(machine, Snacks, ClusterClient!));
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

}