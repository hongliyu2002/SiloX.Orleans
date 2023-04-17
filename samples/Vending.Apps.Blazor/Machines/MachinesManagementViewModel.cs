using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;
using Fluxera.Guards;
using Orleans.FluentResults;
using Orleans.Streams;
using ReactiveUI;
using SiloX.Domain.Abstractions;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Apps.Blazor.Services;
using Vending.Apps.Blazor.Snacks;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;
using Vending.Projection.Abstractions.Machines;
using Vending.Projection.Abstractions.Snacks;

namespace Vending.Apps.Blazor.Machines;

public class MachinesManagementViewModel : ReactiveObject, IActivatableViewModel
{
    private StreamSequenceToken? _machineSequenceToken;
    private StreamSequenceToken? _snackSequenceToken;

    #region Constructor

    /// <inheritdoc />
    public MachinesManagementViewModel(IClusterClientReady clusterClientReady)
    {
        // When the cluster client is ready, set the cluster client.
        ClusterClientReady = Guard.Against.Null(clusterClientReady, nameof(clusterClientReady));
        this.WhenAnyValue(vm => vm.ClusterClientReady)
            .Where(clientReady => clientReady != null)
            .SelectMany(clientReady => clientReady!.ClusterClient.Task)
            .Subscribe(client => ClusterClient = client);

        // Create the cache for the snacks.
        var snacksCache = new SourceCache<SnackViewModel, Guid>(snack => snack.Id);
        snacksCache.Connect()
                   .AutoRefresh(snack => snack.Name)
                   .Sort(SortExpressionComparer<SnackViewModel>.Ascending(snack => snack.Id))
                   .Bind(out var snacks)
                   .Subscribe(set => SnacksChangeSet = set);
        Snacks = snacks;

        // Get snacks and update the cache.
        this.WhenAnyValue(vm => vm.ClusterClient)
            .Where(client => client != null)
            .Select(client => client!.GetGrain<ISnackRetrieverGrain>("Manager"))
            .SelectMany(grain => grain.ListAsync(new SnackRetrieverListQuery(null, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
            .Where(result => result.IsSuccess)
            .Select(result => result.Value)
            .Subscribe(snacksList => snacksCache.AddOrUpdateWith(snacksList));

        // Create the cache for the machines.
        var machinesCache = new SourceCache<MachineViewModel, Guid>(machine => machine.Id);
        var machinesObs = machinesCache.Connect()
                                       .AutoRefresh(machine => machine.MoneyInside)
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
                   .Bind(out var machines)
                   .Subscribe();
        Machines = machines;
        // Recalculate the page count when the cache changes.
        machinesObs.Count()
                   .Subscribe(count =>
                              {
                                  MachineCount = count;
                                  PageCount = (int)Math.Ceiling((double)count / PageSize);
                              });
        this.WhenAnyValue(vm => vm.PageSize)
            .Subscribe(size => PageCount = (int)Math.Ceiling((double)machinesCache.Count / size));

        // Recalculate the page number when the page size or page count changes.
        this.WhenAnyValue(vm => vm.PageSize, vm => vm.PageCount)
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
            .Subscribe(machinesList => machinesCache.AddOrUpdateWith(machinesList));

        // When the current machine changes, if it is null, set the current machine edit view model to null.
        this.WhenAnyValue(vm => vm.CurrentMachine, vm => vm.ClusterClient)
            .Where(tuple => tuple.Item1 == null || tuple.Item2 == null)
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
                                                   .Subscribe(tuple =>
                                                              {
                                                                  _machineSequenceToken = tuple.SequenceToken;
                                                                  var savedEvent = (MachineInfoSavedEvent)tuple.Event;
                                                                  machinesCache.AddOrUpdateWith(savedEvent.Machine);
                                                              })
                                                   .DisposeWith(disposable);
                               allMachinesStreamObs.Where(tuple => tuple.Event is MachineInfoErrorEvent)
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
                                                 .Subscribe(tuple =>
                                                            {
                                                                _snackSequenceToken = tuple.SequenceToken;
                                                                var savedEvent = (SnackInfoSavedEvent)tuple.Event;
                                                                snacksCache.AddOrUpdateWith(savedEvent.Snack);
                                                            })
                                                 .DisposeWith(disposable);
                               allSnacksStreamObs.Where(tuple => tuple.Event is SnackInfoErrorEvent)
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

    #endregion

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

    private MachineEditViewModel? _currentMachineEdit;
    public MachineEditViewModel? CurrentMachineEdit
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

    private IChangeSet<SnackViewModel, Guid>? _snacksChangeSet;
    public IChangeSet<SnackViewModel, Guid>? SnacksChangeSet
    {
        get => _snacksChangeSet;
        set => this.RaiseAndSetIfChanged(ref _snacksChangeSet, value);
    }

    #endregion

    #region Interactions

    /// <summary>
    ///     Interaction that shows the machine edit dialog.
    /// </summary>
    public Interaction<MachineEditViewModel, Unit> ShowEditMachineInteraction { get; } = new();

    /// <summary>
    ///     Interaction that asks the user to confirm the removal of the current machine.
    /// </summary>
    public Interaction<string, bool> ConfirmRemoveMachineInteraction { get; } = new();

    /// <summary>
    ///     Interaction for errors.
    /// </summary>
    public Interaction<IEnumerable<IError>, ErrorRecovery> ErrorsInteraction { get; } = new();

    #endregion

    #region Commands

    /// <summary>
    ///     Gets the command that adds a new machine.
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddMachineCommand { get; }

    private IObservable<bool> CanAddMachine =>
        this.WhenAnyValue(vm => vm.ClusterClient)
            .Select(client => client != null);

    /// <summary>
    ///     Adds a new machine.
    /// </summary>
    private async Task AddMachineAsync()
    {
        bool retry;
        do
        {
            var result = Result.Ok()
                               .Ensure(ClusterClient != null, "No cluster client available.")
                               .MapTry(() =>
                                       {
                                           var machine = new Machine();
                                           machine.Slots.Add(new MachineSlot(machine.Id, 1));
                                           return new MachineEditViewModel(machine, Snacks, ClusterClient!);
                                       });
            if (result.IsSuccess)
            {
                await ShowEditMachineInteraction.Handle(result.Value);
                return;
            }
            var errorRecovery = await ErrorsInteraction.Handle(result.Errors);
            retry = errorRecovery == ErrorRecovery.Retry;
        }
        while (retry);
    }

    /// <summary>
    ///     Gets the command that edits a machine.
    /// </summary>
    public ReactiveCommand<Unit, Unit> EditMachineCommand { get; }

    private IObservable<bool> CanEditMachine =>
        this.WhenAnyValue(vm => vm.CurrentMachine, vm => vm.ClusterClient)
            .Select(tuple => tuple is { Item1: not null, Item2: not null });

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
                                     .Ensure(ClusterClient != null, "No cluster client available.")
                                     .MapTry(() => ClusterClient!.GetGrain<IMachineGrain>(CurrentMachine!.Id))
                                     .MapTryAsync(grain => grain.GetMachineAsync())
                                     .TapTryAsync(machine => CurrentMachineEdit = new MachineEditViewModel(machine, Snacks, ClusterClient!));
            if (result.IsSuccess)
            {
                await ShowEditMachineInteraction.Handle(CurrentMachineEdit!);
                return;
            }
            var errorRecovery = await ErrorsInteraction.Handle(result.Errors);
            retry = errorRecovery == ErrorRecovery.Retry;
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
    ///     Removes the current machine.
    /// </summary>
    private async Task RemoveMachineAsync()
    {
        var confirm = await ConfirmRemoveMachineInteraction.Handle(CurrentMachine!.Id.ToString());
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
            var errorRecovery = await ErrorsInteraction.Handle(result.Errors);
            retry = errorRecovery == ErrorRecovery.Retry;
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
            var errorRecovery = await ErrorsInteraction.Handle(result.Errors);
            retry = errorRecovery == ErrorRecovery.Retry;
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