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
using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using Orleans;
using Orleans.FluentResults;
using Orleans.Runtime;
using Orleans.Streams;
using ReactiveUI;
using Vending.App.Snacks;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;
using Vending.Projection.Abstractions.Snacks;

namespace Vending.App.Machines;

public class MachineEditViewModel : ReactiveObject, IActivatableViewModel, IOrleansObject
{
    private readonly SourceCache<SnackViewModel, Guid> _snacksCache;
    private readonly SourceCache<SlotEditViewModel, int> _slotsCache;
    private readonly ReadOnlyObservableCollection<SlotEditViewModel> _slots;

    private StreamSubscriptionHandle<MachineEvent>? _subscription;
    private StreamSequenceToken? _lastSequenceToken;

    public MachineEditViewModel(Machine machine, IClusterClient clusterClient)
    {
        Guard.Against.Null(machine, nameof(machine));
        ClusterClient = Guard.Against.Null(clusterClient, nameof(clusterClient));

        // Create the cache for the slots.
        _slotsCache = new SourceCache<SlotEditViewModel, int>(slot => slot.Position);
        _slotsCache.Connect()
                   .Sort(SortExpressionComparer<SlotEditViewModel>.Ascending(slot => slot.Position))
                   .ObserveOn(RxApp.MainThreadScheduler)
                   .Bind(out _slots)
                   .Subscribe();
        // Create the cache for the snacks.
        _snacksCache = new SourceCache<SnackViewModel, Guid>(snack => snack.Id);
        // Get snacks and update the cache.
        this.WhenAnyValue(vm => vm.ClusterClient)
            .Where(client => client != null)
            .Select(client => client!.GetGrain<ISnackRetrieverGrain>("Manager"))
            .SelectMany(grain => grain.ListAsync(new SnackRetrieverListQuery(new Dictionary<string, bool> { { "Id", false } }, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
            .Where(result => result.IsSuccess)
            .Select(result => result.Value)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(snacks => _snacksCache.Edit(updater => updater.AddOrUpdate(snacks.Select(snack => new SnackViewModel(snack)))));
        // Recreate the money inside when any of the money properties change.
        this.WhenAnyValue(vm => vm.MoneyYuan1, vm => vm.MoneyYuan2, vm => vm.MoneyYuan5, vm => vm.MoneyYuan10, vm => vm.MoneyYuan20, vm => vm.MoneyYuan50, vm => vm.MoneyYuan100)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(tuple =>
                       {
                           MoneyInside = new Money(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7);
                           MoneyAmount = MoneyInside.Amount;
                       });
        this.WhenActivated(disposable =>
                           {
                               // When the cluster client changes, subscribe to the machine info stream.
                               this.WhenAnyValue(vm => vm.Id, vm => vm.ClusterClient)
                                   .Where(tuple => tuple.Item1 != Guid.Empty && tuple.Item2 != null)
                                   .Select(tuple => (tuple.Item1, tuple.Item2!.GetStreamProvider(Constants.StreamProviderName)))
                                   .Select(tuple => tuple.Item2.GetStream<MachineEvent>(StreamId.Create(Constants.MachinesNamespace, tuple.Item1)))
                                   .SelectMany(stream => stream.SubscribeAsync(HandleEventAsync, HandleErrorAsync, HandleCompletedAsync, _lastSequenceToken))
                                   .ObserveOn(RxApp.MainThreadScheduler)
                                   .Subscribe(HandleSubscriptionAsync)
                                   .DisposeWith(disposable);
                               Disposable.Create(HandleSubscriptionDisposeAsync)
                                         .DisposeWith(disposable);
                           });
        // Create the commands.
        AddSlotCommand = ReactiveCommand.Create(AddSlot);
        RemoveSlotCommand = ReactiveCommand.CreateFromTask(RemoveSlotAsync, CanRemoveSlot);
        SaveMachineCommand = ReactiveCommand.CreateFromTask(SaveMachineAsync, CanSaveMachine);
        CloseCommand = ReactiveCommand.CreateFromTask(CloseAsync);
        // Load the machine.
        LoadMachine(machine);
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

    private Guid _id;
    public Guid Id
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    private Money _moneyInside = Money.Zero;
    public Money MoneyInside
    {
        get => _moneyInside;
        set => this.RaiseAndSetIfChanged(ref _moneyInside, value);
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

    private bool _isDeleted;
    public bool IsDeleted
    {
        get => _isDeleted;
        set => this.RaiseAndSetIfChanged(ref _isDeleted, value);
    }

    private SlotEditViewModel? _currentSlot;
    public SlotEditViewModel? CurrentSlot
    {
        get => _currentSlot;
        set => this.RaiseAndSetIfChanged(ref _currentSlot, value);
    }

    public ReadOnlyObservableCollection<SlotEditViewModel> Slots => _slots;

    #endregion

    #region Commands

    /// <summary>
    ///     Gets the command that adds a new slot.
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddSlotCommand { get; }

    /// <summary>
    ///     Adds a new slot.
    /// </summary>
    private void AddSlot()
    {
        var position = _slotsCache.Keys.Max() + 1;
        _slotsCache.Edit(updater => updater.AddOrUpdate(new SlotEditViewModel(new MachineSlot(Id, position), _snacksCache)));
    }

    /// <summary>
    ///     Gets the command that removes the current slot.
    /// </summary>
    public ReactiveCommand<Unit, Unit> RemoveSlotCommand { get; }

    /// <summary>
    ///     Gets the observable that indicates whether the remove slot command can be executed.
    /// </summary>
    private IObservable<bool> CanRemoveSlot =>
        this.WhenAnyValue(vm => vm.CurrentSlot)
            .Select(slot => slot != null);

    /// <summary>
    ///     Gets the interaction that asks the user to confirm the removal of the current slot.
    /// </summary>
    public Interaction<string, bool> ConfirmRemoveSlot { get; } = new();

    /// <summary>
    ///     Removes the current slot.
    /// </summary>
    private async Task RemoveSlotAsync()
    {
        var confirm = await ConfirmRemoveSlot.Handle(CurrentSlot!.Position.ToString());
        if (confirm)
        {
            _slotsCache.Edit(updater => updater.Remove(CurrentSlot!.Position));
        }
    }

    /// <summary>
    ///     Gets the command to save the machine.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SaveMachineCommand { get; }

    private IObservable<bool> CanSaveMachine =>
        this.WhenAnyValue(vm => vm.Slots, vm => vm.IsDeleted, vm => vm.ClusterClient)
            .Select(tuple => tuple.Item1.IsNotNullOrEmpty()
                          && tuple.Item1.GroupBy(slot => slot.Position)
                                  .All(g => g.Count() == 1)
                          && tuple is { Item2: false, Item3: not null });

    private async Task SaveMachineAsync()
    {
        bool retry;
        do
        {
            var slots = Slots.ToDictionary(slot => slot.Position, slot => slot.SnackPile);
            IMachineRepoGrain repoGrain = null!;
            var result = await Result.Ok()
                                     .MapTry(() => repoGrain = ClusterClient!.GetGrain<IMachineRepoGrain>("Manager"))
                                     .BindTryIfAsync(Id == Guid.Empty, () => repoGrain.CreateAsync(new MachineRepoCreateCommand(MoneyInside, slots, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
                                     .BindTryIfAsync<Machine>(Id != Guid.Empty, () => repoGrain.UpdateAsync(new MachineRepoUpdateCommand(Id, MoneyInside, slots, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
                                     .TapTryAsync(LoadMachine);
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
    ///     Gets the command to close the window.
    /// </summary>
    public ReactiveCommand<Unit, Unit> CloseCommand { get; }

    private async Task CloseAsync()
    {
        // await Interactions.Machines.Close(this);
    }

    #endregion

    #region Load Machine

    private void LoadMachine(Machine machine)
    {
        Id = machine.Id;
        MoneyYuan1 = machine.MoneyInside.Yuan1;
        MoneyYuan2 = machine.MoneyInside.Yuan2;
        MoneyYuan5 = machine.MoneyInside.Yuan5;
        MoneyYuan10 = machine.MoneyInside.Yuan10;
        MoneyYuan20 = machine.MoneyInside.Yuan20;
        MoneyYuan50 = machine.MoneyInside.Yuan50;
        MoneyYuan100 = machine.MoneyInside.Yuan100;
        MoneyAmount = machine.MoneyInside.Amount;
        IsDeleted = machine.IsDeleted;
        _slotsCache.Edit(updater => updater.Load(machine.Slots.Select(slot => new SlotEditViewModel(slot, _snacksCache))));
    }

    #endregion

    #region Stream Handlers

    private async void HandleSubscriptionAsync(StreamSubscriptionHandle<MachineEvent> subscription)
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

    private Task HandleEventAsync(MachineEvent domainEvent, StreamSequenceToken sequenceToken)
    {
        _lastSequenceToken = sequenceToken;
        return domainEvent switch
               {
                   MachineInitializedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineDeletedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineUpdatedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineSlotAddedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineSlotRemovedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineMoneyLoadedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineMoneyUnloadedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineMoneyInsertedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineMoneyReturnedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineSnacksLoadedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineSnacksUnloadedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineSnackBoughtEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineErrorEvent machineEvent => ApplyErrorEventAsync(machineEvent),
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

    private Task ApplyEventAsync(MachineInitializedEvent machineEvent)
    {
        if (machineEvent.MachineId != Id)
        {
            return Task.CompletedTask;
        }
        var dispatcher = Application.Current.Dispatcher;
        dispatcher?.Invoke(() =>
                           {
                               MoneyYuan1 = machineEvent.MoneyInside.Yuan1;
                               MoneyYuan2 = machineEvent.MoneyInside.Yuan2;
                               MoneyYuan5 = machineEvent.MoneyInside.Yuan5;
                               MoneyYuan10 = machineEvent.MoneyInside.Yuan10;
                               MoneyYuan20 = machineEvent.MoneyInside.Yuan20;
                               MoneyYuan50 = machineEvent.MoneyInside.Yuan50;
                               MoneyYuan100 = machineEvent.MoneyInside.Yuan100;
                               MoneyAmount = machineEvent.MoneyInside.Amount;
                           });
        _slotsCache.Edit(updater => updater.Load(machineEvent.Slots.Select(slot => new SlotEditViewModel(slot, _snacksCache))));
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineDeletedEvent machineEvent)
    {
        if (machineEvent.MachineId != Id)
        {
            return Task.CompletedTask;
        }
        var dispatcher = Application.Current.Dispatcher;
        dispatcher?.Invoke(() =>
                           {
                               MoneyYuan1 = machineEvent.MoneyInside.Yuan1;
                               MoneyYuan2 = machineEvent.MoneyInside.Yuan2;
                               MoneyYuan5 = machineEvent.MoneyInside.Yuan5;
                               MoneyYuan10 = machineEvent.MoneyInside.Yuan10;
                               MoneyYuan20 = machineEvent.MoneyInside.Yuan20;
                               MoneyYuan50 = machineEvent.MoneyInside.Yuan50;
                               MoneyYuan100 = machineEvent.MoneyInside.Yuan100;
                               MoneyAmount = machineEvent.MoneyInside.Amount;
                               IsDeleted = true;
                           });
        _slotsCache.Edit(updater => updater.Load(machineEvent.Slots.Select(slot => new SlotEditViewModel(slot, _snacksCache))));
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineUpdatedEvent machineEvent)
    {
        if (machineEvent.MachineId != Id)
        {
            return Task.CompletedTask;
        }
        var dispatcher = Application.Current.Dispatcher;
        dispatcher?.Invoke(() =>
                           {
                               MoneyYuan1 = machineEvent.MoneyInside.Yuan1;
                               MoneyYuan2 = machineEvent.MoneyInside.Yuan2;
                               MoneyYuan5 = machineEvent.MoneyInside.Yuan5;
                               MoneyYuan10 = machineEvent.MoneyInside.Yuan10;
                               MoneyYuan20 = machineEvent.MoneyInside.Yuan20;
                               MoneyYuan50 = machineEvent.MoneyInside.Yuan50;
                               MoneyYuan100 = machineEvent.MoneyInside.Yuan100;
                               MoneyAmount = machineEvent.MoneyInside.Amount;
                           });
        _slotsCache.Edit(updater => updater.Load(machineEvent.Slots.Select(slot => new SlotEditViewModel(slot, _snacksCache))));
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineSlotAddedEvent machineEvent)
    {
        if (machineEvent.MachineId != Id)
        {
            return Task.CompletedTask;
        }
        _slotsCache.Edit(updater => updater.AddOrUpdate(new SlotEditViewModel(machineEvent.Slot, _snacksCache)));
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineSlotRemovedEvent machineEvent)
    {
        if (machineEvent.MachineId != Id)
        {
            return Task.CompletedTask;
        }
        _slotsCache.Edit(updater => updater.Remove(machineEvent.Position));
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineMoneyLoadedEvent machineEvent)
    {
        if (machineEvent.MachineId != Id)
        {
            return Task.CompletedTask;
        }
        var dispatcher = Application.Current.Dispatcher;
        dispatcher?.Invoke(() =>
                           {
                               MoneyYuan1 = machineEvent.MoneyInside.Yuan1;
                               MoneyYuan2 = machineEvent.MoneyInside.Yuan2;
                               MoneyYuan5 = machineEvent.MoneyInside.Yuan5;
                               MoneyYuan10 = machineEvent.MoneyInside.Yuan10;
                               MoneyYuan20 = machineEvent.MoneyInside.Yuan20;
                               MoneyYuan50 = machineEvent.MoneyInside.Yuan50;
                               MoneyYuan100 = machineEvent.MoneyInside.Yuan100;
                               MoneyAmount = machineEvent.MoneyInside.Amount;
                           });
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineMoneyUnloadedEvent machineEvent)
    {
        if (machineEvent.MachineId != Id)
        {
            return Task.CompletedTask;
        }
        var dispatcher = Application.Current.Dispatcher;
        dispatcher?.Invoke(() =>
                           {
                               MoneyYuan1 = machineEvent.MoneyInside.Yuan1;
                               MoneyYuan2 = machineEvent.MoneyInside.Yuan2;
                               MoneyYuan5 = machineEvent.MoneyInside.Yuan5;
                               MoneyYuan10 = machineEvent.MoneyInside.Yuan10;
                               MoneyYuan20 = machineEvent.MoneyInside.Yuan20;
                               MoneyYuan50 = machineEvent.MoneyInside.Yuan50;
                               MoneyYuan100 = machineEvent.MoneyInside.Yuan100;
                               MoneyAmount = machineEvent.MoneyInside.Amount;
                           });
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineMoneyInsertedEvent machineEvent)
    {
        if (machineEvent.MachineId != Id)
        {
            return Task.CompletedTask;
        }
        var dispatcher = Application.Current.Dispatcher;
        dispatcher?.Invoke(() =>
                           {
                               MoneyYuan1 = machineEvent.MoneyInside.Yuan1;
                               MoneyYuan2 = machineEvent.MoneyInside.Yuan2;
                               MoneyYuan5 = machineEvent.MoneyInside.Yuan5;
                               MoneyYuan10 = machineEvent.MoneyInside.Yuan10;
                               MoneyYuan20 = machineEvent.MoneyInside.Yuan20;
                               MoneyYuan50 = machineEvent.MoneyInside.Yuan50;
                               MoneyYuan100 = machineEvent.MoneyInside.Yuan100;
                               MoneyAmount = machineEvent.MoneyInside.Amount;
                           });
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineMoneyReturnedEvent machineEvent)
    {
        if (machineEvent.MachineId != Id)
        {
            return Task.CompletedTask;
        }
        var dispatcher = Application.Current.Dispatcher;
        dispatcher?.Invoke(() =>
                           {
                               MoneyYuan1 = machineEvent.MoneyInside.Yuan1;
                               MoneyYuan2 = machineEvent.MoneyInside.Yuan2;
                               MoneyYuan5 = machineEvent.MoneyInside.Yuan5;
                               MoneyYuan10 = machineEvent.MoneyInside.Yuan10;
                               MoneyYuan20 = machineEvent.MoneyInside.Yuan20;
                               MoneyYuan50 = machineEvent.MoneyInside.Yuan50;
                               MoneyYuan100 = machineEvent.MoneyInside.Yuan100;
                               MoneyAmount = machineEvent.MoneyInside.Amount;
                           });
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineSnacksLoadedEvent machineEvent)
    {
        if (machineEvent.MachineId != Id)
        {
            return Task.CompletedTask;
        }
        _slotsCache.Edit(updater => updater.AddOrUpdate(new SlotEditViewModel(machineEvent.Slot, _snacksCache)));
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineSnacksUnloadedEvent machineEvent)
    {
        if (machineEvent.MachineId != Id)
        {
            return Task.CompletedTask;
        }
        _slotsCache.Edit(updater => updater.AddOrUpdate(new SlotEditViewModel(machineEvent.Slot, _snacksCache)));
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineSnackBoughtEvent machineEvent)
    {
        if (machineEvent.MachineId != Id)
        {
            return Task.CompletedTask;
        }
        _slotsCache.Edit(updater => updater.AddOrUpdate(new SlotEditViewModel(machineEvent.Slot, _snacksCache)));
        return Task.CompletedTask;
    }

    private Task ApplyErrorEventAsync(MachineErrorEvent errorEvent)
    {
        return Task.CompletedTask;
    }

    #endregion

}