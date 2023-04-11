using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using Orleans;
using Orleans.FluentResults;
using Orleans.Runtime;
using Orleans.Streams;
using ReactiveUI;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;
using Vending.Projection.Abstractions.Machines;

namespace Vending.App.Machines;

public class MachinesManagementViewModel : ReactiveObject, IActivatableViewModel, IOrleansObject
{
    private readonly SourceCache<MachineViewModel, Guid> _machinesCache;
    private readonly ReadOnlyObservableCollection<MachineViewModel> _machines;
    private StreamSubscriptionHandle<MachineInfoEvent>? _subscription;
    private StreamSequenceToken? _lastSequenceToken;

    /// <inheritdoc />
    public MachinesManagementViewModel()
    {
        // Create the cache for the machines.
        _machinesCache = new SourceCache<MachineViewModel, Guid>(machine => machine.Id);
        // Connect the cache to the machines observable collection.
        _machinesCache.Connect()
                      .Filter(this.WhenAnyValue(vm => vm.MoneyAmountStart, vm => vm.MoneyAmountEnd)
                                  .Throttle(TimeSpan.FromMilliseconds(500))
                                  .DistinctUntilChanged()
                                  .Select(_ => new Func<MachineViewModel, bool>(machine => (MoneyAmountStart == null || machine.MoneyInside.Amount >= MoneyAmountStart)
                                                                                        && (MoneyAmountEnd == null || machine.MoneyInside.Amount < MoneyAmountEnd)
                                                                                        && machine.IsDeleted == false)))
                      .Sort(SortExpressionComparer<MachineViewModel>.Ascending(machine => machine.Id))
                      .Page(this.WhenAnyValue(vm => vm.PageNumber, vm => vm.PageSize)
                                .DistinctUntilChanged()
                                .Select(_ => new PageRequest(PageNumber, PageSize)))
                      .ObserveOn(RxApp.MainThreadScheduler)
                      .Bind(out _machines)
                      .Subscribe();
        // Recalculate the page count when the cache changes.
        _machinesCache.CountChanged.ObserveOn(RxApp.MainThreadScheduler)
                      .Subscribe(count => PageCount = (int)Math.Ceiling((double)count / PageSize));
        this.WhenAnyValue(vm => vm.PageSize)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => PageCount = (int)Math.Ceiling((double)_machinesCache.Count / PageSize));
        // Recalculate the page number when the page size or page count changes.
        this.WhenAnyValue(vm => vm.PageSize, vm => vm.PageCount)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ =>
                       {
                           var pageNumber = PageNumber <= 0 ? 1 : PageNumber;
                           var oldOffset = _oldPageSize * (pageNumber - 1) + 1;
                           var newPageNumber = (int)Math.Ceiling((double)oldOffset / PageSize);
                           pageNumber = Math.Min(newPageNumber, PageCount);
                           PageNumber = pageNumber <= 0 ? 1 : pageNumber;
                       });
        // Get machines and update the cache.
        this.WhenAnyValue(vm => vm.MoneyAmountStart, vm => vm.MoneyAmountEnd, vm => vm.ClusterClient)
            .Where(_ => ClusterClient != null)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .DistinctUntilChanged()
            .Select(_ => ClusterClient!.GetGrain<IMachineRetrieverGrain>("Manager"))
            .SelectMany(grain => grain.ListAsync(new MachineRetrieverListQuery(null, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager", new DecimalRange(MoneyAmountStart, MoneyAmountEnd))))
            .Where(result => result.IsSuccess)
            .Select(result => result.Value)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(machines => _machinesCache.Edit(updater => updater.AddOrUpdate(machines.Select(machine => new MachineViewModel(machine)))));
        // When the current machine changes, get the machine edit view model.
        this.WhenAnyValue(vm => vm.CurrentMachine, vm => vm.ClusterClient)
            .Where(_ => CurrentMachine != null && ClusterClient != null)
            .Select(_ => ClusterClient!.GetGrain<IMachineGrain>(CurrentMachine!.Id))
            .SelectMany(grain => grain.GetMachineAsync())
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(machine => CurrentMachineEdit = new MachineEditWindowModel(machine, ClusterClient!));
        this.WhenActivated(disposable =>
                           {
                               // When the cluster client changes, subscribe to the machine info stream.
                               this.WhenAnyValue(vm => vm.ClusterClient)
                                   .Where(_ => ClusterClient != null)
                                   .Select(_ => ClusterClient!.GetStreamProvider(Constants.StreamProviderName))
                                   .Select(streamProvider => streamProvider.GetStream<MachineInfoEvent>(StreamId.Create(Constants.MachineInfosBroadcastNamespace, Guid.Empty)))
                                   .SelectMany(stream => stream.SubscribeAsync(HandleEventAsync, HandleErrorAsync, HandleCompletedAsync, _lastSequenceToken))
                                   .Subscribe(HandleSubscriptionAsync)
                                   .DisposeWith(disposable);
                               Disposable.Create(HandleSubscriptionDisposeAsync)
                                         .DisposeWith(disposable);
                           });
        // Create the commands.
        AddMachineCommand = ReactiveCommand.CreateFromTask(AddMachineAsync, CanAddMachine);
        EditMachineCommand = ReactiveCommand.CreateFromTask(EditMachineAsync, CanEditMachine);
        RemoveMachineCommand = ReactiveCommand.CreateFromTask(RemoveMachineAsync, CanRemoveMachine);
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

    public ReadOnlyObservableCollection<MachineViewModel> Machines => _machines;

    #endregion

    #region Commands

    /// <summary>
    ///     Gets the command that adds a new machine.
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddMachineCommand { get; }

    private IObservable<bool> CanAddMachine =>
        this.WhenAnyValue(vm => vm.ClusterClient)
            .Select(_ => ClusterClient != null);

    /// <summary>
    ///     Gets the interaction that shows the machine edit dialog.
    /// </summary>
    public Interaction<MachineEditWindowModel, Unit> ShowEditMachine { get; } = new();

    /// <summary>
    ///     Adds a new machine.
    /// </summary>
    private async Task AddMachineAsync()
    {
        var machine = new Machine();
        machine.Slots.Add(new MachineSlot(machine.Id, 1));
        var windowModel = new MachineEditWindowModel(machine, ClusterClient!);
        await ShowEditMachine.Handle(windowModel);
    }

    /// <summary>
    ///     Gets the command that edits a machine.
    /// </summary>
    public ReactiveCommand<Unit, Unit> EditMachineCommand { get; }

    private IObservable<bool> CanEditMachine =>
        this.WhenAnyValue(vm => vm.CurrentMachineEdit)
            .Select(_ => CurrentMachineEdit != null && ClusterClient != null);

    /// <summary>
    ///     Edits the current machine.
    /// </summary>
    private async Task EditMachineAsync()
    {
        await ShowEditMachine.Handle(CurrentMachineEdit!);
    }

    /// <summary>
    ///     Gets the command that removes the current machine.
    /// </summary>
    public ReactiveCommand<Unit, Unit> RemoveMachineCommand { get; }

    /// <summary>
    ///     Gets the observable that indicates whether the remove machine command can be executed.
    /// </summary>
    private IObservable<bool> CanRemoveMachine =>
        this.WhenAnyValue(vm => vm.CurrentMachine)
            .Select(_ => CurrentMachine != null && ClusterClient != null);

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
    ///     Gets the command that moves to the previous page.
    /// </summary>
    public ReactiveCommand<Unit, Unit> GoPreviousPageCommand { get; }

    private IObservable<bool> CanGoPreviousPage =>
        this.WhenAnyValue(vm => vm.PageNumber)
            .Select(_ => PageNumber > 1);

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
            .Select(_ => PageNumber < PageCount);

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

}