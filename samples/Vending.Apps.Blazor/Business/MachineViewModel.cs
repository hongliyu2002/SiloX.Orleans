using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Fluxera.Guards;
using Orleans.FluentResults;
using Orleans.Streams;
using ReactiveUI;
using SiloX.Domain.Abstractions.Extensions;
using Splat;
using Vending.Apps.Blazor.Services;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;

namespace Vending.Apps.Blazor.Business;

public class MachineViewModel : ReactiveObject, IActivatableViewModel
{
    private readonly SourceCache<SlotViewModel, int> _slotsCache;

    private StreamSequenceToken? _lastSequenceToken;

    #region Constructor

    /// <inheritdoc />
    public MachineViewModel(Guid machineId)
    {
        Id = Guard.Against.Null(machineId, nameof(machineId));

        // When the cluster client is ready, set the cluster client.
        ClusterClientReady = Locator.Current.GetService<IClusterClientReady>();
        this.WhenAnyValue(vm => vm.ClusterClientReady)
            .Where(clientReady => clientReady != null)
            .SelectMany(clientReady => clientReady!.ClusterClient.Task)
            .Subscribe(client => ClusterClient = client);

        // Load the machine and update the view model.
        this.WhenAnyValue(vm => vm.Id, vm => vm.ClusterClient)
            .Where(tuple => tuple.Item1 != Guid.Empty && tuple.Item2 != null)
            .DistinctUntilChanged()
            .Select(tuple => tuple.Item2!.GetGrain<IMachineGrain>(tuple.Item1))
            .SelectMany(grain => grain.GetMachineAsync())
            .Subscribe(UpdateWith);

        // Create the cache for the slots.
        _slotsCache = new SourceCache<SlotViewModel, int>(slot => slot.Position);
        _slotsCache.Connect()
                   .AutoRefresh(slot => slot.Position)
                   .Sort(SortExpressionComparer<SlotViewModel>.Ascending(slot => slot.Position))
                   .Bind(out var slots)
                   .Subscribe(set => SlotsChangeSet = set);
        Slots = slots;

        // Stream events subscription.
        this.WhenActivated(disposable =>
                           {
                               // When the cluster client changes, subscribe to the machine info stream.
                               var machineStreamObs = this.WhenAnyValue(vm => vm.Id, vm => vm.ClusterClient)
                                                          .Where(tuple => tuple.Item1 != Guid.Empty && tuple.Item2 != null)
                                                          .DistinctUntilChanged()
                                                          .SelectMany(tuple => tuple.Item2!.GetSubscriberStreamWithGuidKey<MachineEvent>(Constants.StreamProviderName, Constants.MachinesNamespace, tuple.Item1, _lastSequenceToken))
                                                          .Publish()
                                                          .RefCount();
                               machineStreamObs.Where(tuple => tuple.Event is MachineInitializedEvent)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineInitializedEvent)tuple.Event;
                                                              UpdateWith(machineEvent.MoneyInside);
                                                              _slotsCache.LoadWith(machineEvent.Slots);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineDeletedEvent)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineDeletedEvent)tuple.Event;
                                                              UpdateWith(machineEvent.MoneyInside);
                                                              IsDeleted = true;
                                                              _slotsCache.LoadWith(machineEvent.Slots);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineUpdatedEvent)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineUpdatedEvent)tuple.Event;
                                                              UpdateWith(machineEvent.MoneyInside);
                                                              _slotsCache.LoadWith(machineEvent.Slots);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineSlotAddedEvent)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineSlotAddedEvent)tuple.Event;
                                                              _slotsCache.AddOrUpdateWith(machineEvent.Slot);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineSlotRemovedEvent)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineSlotRemovedEvent)tuple.Event;
                                                              _slotsCache.RemoveWith(machineEvent.Slot);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineMoneyLoadedEvent)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineMoneyLoadedEvent)tuple.Event;
                                                              UpdateWith(machineEvent.MoneyInside);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineMoneyUnloadedEvent)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineMoneyUnloadedEvent)tuple.Event;
                                                              UpdateWith(machineEvent.MoneyInside);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineMoneyInsertedEvent)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineMoneyInsertedEvent)tuple.Event;
                                                              UpdateWith(machineEvent.MoneyInside);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineMoneyReturnedEvent)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineMoneyReturnedEvent)tuple.Event;
                                                              UpdateWith(machineEvent.MoneyInside);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineSnacksLoadedEvent)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineSnacksLoadedEvent)tuple.Event;
                                                              _slotsCache.AddOrUpdateWith(machineEvent.Slot);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineSnacksUnloadedEvent)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineSnacksUnloadedEvent)tuple.Event;
                                                              _slotsCache.AddOrUpdateWith(machineEvent.Slot);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineSnackBoughtEvent)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineSnackBoughtEvent)tuple.Event;
                                                              _slotsCache.AddOrUpdateWith(machineEvent.Slot);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineErrorEvent)
                                               .Subscribe(tuple =>
                                                          {
                                                              var errorEvent = (MachineErrorEvent)tuple.Event;
                                                              ErrorInfo = $"{errorEvent.Code}:{string.Join("\n", errorEvent.Reasons)}";
                                                          })
                                               .DisposeWith(disposable);
                           });
        // Create the commands.
        InsertMoneyCommand = ReactiveCommand.CreateFromTask<Money, bool>(InsertMoneyAsync, CanInsertMoney);
        ReturnMoneyCommand = ReactiveCommand.CreateFromTask(ReturnMoneyAsync, CanReturnMoney);
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

    private Guid _id;
    public Guid Id
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    private int _moneyYuan1;
    public int MoneyYuan1
    {
        get => _moneyYuan1;
        set => this.RaiseAndSetIfChanged(ref _moneyYuan1, value);
    }

    private int _moneyYuan2;
    public int MoneyYuan2
    {
        get => _moneyYuan2;
        set => this.RaiseAndSetIfChanged(ref _moneyYuan2, value);
    }

    private int _moneyYuan5;
    public int MoneyYuan5
    {
        get => _moneyYuan5;
        set => this.RaiseAndSetIfChanged(ref _moneyYuan5, value);
    }

    private int _moneyYuan10;
    public int MoneyYuan10
    {
        get => _moneyYuan10;
        set => this.RaiseAndSetIfChanged(ref _moneyYuan10, value);
    }

    private int _moneyYuan20;
    public int MoneyYuan20
    {
        get => _moneyYuan20;
        set => this.RaiseAndSetIfChanged(ref _moneyYuan20, value);
    }

    private int _moneyYuan50;
    public int MoneyYuan50
    {
        get => _moneyYuan50;
        set => this.RaiseAndSetIfChanged(ref _moneyYuan50, value);
    }

    private int _moneyYuan100;
    public int MoneyYuan100
    {
        get => _moneyYuan100;
        set => this.RaiseAndSetIfChanged(ref _moneyYuan100, value);
    }

    private decimal _moneyAmount;
    public decimal MoneyAmount
    {
        get => _moneyAmount;
        set => this.RaiseAndSetIfChanged(ref _moneyAmount, value);
    }

    private decimal _amountInTransaction;
    public decimal AmountInTransaction
    {
        get => _amountInTransaction;
        set => this.RaiseAndSetIfChanged(ref _amountInTransaction, value);
    }

    private bool _isDeleted;
    public bool IsDeleted
    {
        get => _isDeleted;
        set => this.RaiseAndSetIfChanged(ref _isDeleted, value);
    }

    public ReadOnlyObservableCollection<SlotViewModel> Slots { get; }

    private IChangeSet<SlotViewModel, int>? _slotsChangeSet;
    public IChangeSet<SlotViewModel, int>? SlotsChangeSet
    {
        get => _slotsChangeSet;
        set => this.RaiseAndSetIfChanged(ref _slotsChangeSet, value);
    }

    #endregion

    #region Interactions

    /// <summary>
    ///     Interaction for errors.
    /// </summary>
    public Interaction<IEnumerable<IError>, ErrorRecovery> ErrorsInteraction { get; } = new();

    #endregion

    #region Load Machine

    public void UpdateWith(Machine machine)
    {
        Id = machine.Id;
        UpdateWith(machine.MoneyInside);
        AmountInTransaction = machine.AmountInTransaction;
        IsDeleted = machine.IsDeleted;
        _slotsCache.LoadWith(machine.Slots);
    }

    public void UpdateWith(Money money)
    {
        MoneyYuan1 = money.Yuan1;
        MoneyYuan2 = money.Yuan2;
        MoneyYuan5 = money.Yuan5;
        MoneyYuan10 = money.Yuan10;
        MoneyYuan20 = money.Yuan20;
        MoneyYuan50 = money.Yuan50;
        MoneyYuan100 = money.Yuan100;
        MoneyAmount = money.Amount;
    }

    #endregion

    #region Commands

    #region Insert Money

    /// <summary>
    ///     Gets the command that inserts money.
    /// </summary>
    public ReactiveCommand<Money, bool> InsertMoneyCommand { get; }

    private IObservable<bool> CanInsertMoney =>
        this.WhenAnyValue(vm => vm.Id, vm => vm.ClusterClient)
            .Select(tuple => tuple.Item1 != Guid.Empty && tuple.Item2 != null);

    /// <summary>
    ///     Inserts money to the current machine.
    /// </summary>
    private async Task<bool> InsertMoneyAsync(Money money)
    {
        bool retry;
        do
        {
            var result = await Result.Ok()
                                     .Ensure(Id != Guid.Empty, "No machine id.")
                                     .Ensure(ClusterClient != null, "No cluster client available.")
                                     .MapTry(() => ClusterClient!.GetGrain<IMachineGrain>(Id))
                                     .MapTryAsync(grain => grain.InsertMoneyAsync(new MachineInsertMoneyCommand(money, Guid.NewGuid(), DateTimeOffset.UtcNow, "User")));
            if (result.IsSuccess)
            {
                return true;
            }
            var errorRecovery = await ErrorsInteraction.Handle(result.Errors);
            retry = errorRecovery == ErrorRecovery.Retry;
        }
        while (retry);
        return false;
    }

    #endregion

    #region Return Money

    /// <summary>
    ///     Gets the command that returns money.
    /// </summary>
    public ReactiveCommand<Unit, bool> ReturnMoneyCommand { get; }

    private IObservable<bool> CanReturnMoney =>
        this.WhenAnyValue(vm => vm.Id, vm => vm.ClusterClient)
            .Select(tuple => tuple.Item1 != Guid.Empty && tuple.Item2 != null);

    /// <summary>
    ///     Returns money to the current machine.
    /// </summary>
    private async Task<bool> ReturnMoneyAsync()
    {
        bool retry;
        do
        {
            var result = await Result.Ok()
                                     .Ensure(Id != Guid.Empty, "No machine id.")
                                     .Ensure(ClusterClient != null, "No cluster client available.")
                                     .MapTry(() => ClusterClient!.GetGrain<IMachineGrain>(Id))
                                     .MapTryAsync(grain => grain.ReturnMoneyAsync(new MachineReturnMoneyCommand(Guid.NewGuid(), DateTimeOffset.UtcNow, "User")));
            if (result.IsSuccess)
            {
                return true;
            }
            var errorRecovery = await ErrorsInteraction.Handle(result.Errors);
            retry = errorRecovery == ErrorRecovery.Retry;
        }
        while (retry);
        return false;
    }

    #endregion

    #endregion

}