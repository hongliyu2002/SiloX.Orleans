﻿using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using Orleans.FluentResults;
using Orleans.Streams;
using ReactiveUI;
using SiloX.Domain.Abstractions.Extensions;
using Vending.App.Wpf.Snacks;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;

namespace Vending.App.Wpf.Machines;

public class MachineEditWindowModel : ReactiveObject, IActivatableViewModel, IOrleansObject
{
    private readonly SourceCache<SlotEditViewModel, int> _slotsCache;

    private StreamSequenceToken? _lastSequenceToken;

    public MachineEditWindowModel(Machine machine, ReadOnlyObservableCollection<SnackViewModel> snacks, IClusterClient clusterClient)
    {
        Guard.Against.Null(machine, nameof(machine));
        _snacks = Guard.Against.Null(snacks, nameof(snacks));
        ClusterClient = Guard.Against.Null(clusterClient, nameof(clusterClient));
        // Create the cache for the slots.
        _slotsCache = new SourceCache<SlotEditViewModel, int>(slot => slot.Position);
        _slotsCache.Connect()
                   .Sort(SortExpressionComparer<SlotEditViewModel>.Ascending(slot => slot.Position))
                   .ObserveOn(RxApp.MainThreadScheduler)
                   .Bind(out _slots)
                   .Subscribe();

        // Recalculate the slot count when the slots change.
        _slotsCache.CountChanged.ObserveOn(RxApp.MainThreadScheduler)
                   .Subscribe(count => SlotCount = count);

        // Recreate the money inside when any of the money properties change.
        this.WhenAnyValue(vm => vm.MoneyYuan1, vm => vm.MoneyYuan2, vm => vm.MoneyYuan5, vm => vm.MoneyYuan10, vm => vm.MoneyYuan20, vm => vm.MoneyYuan50, vm => vm.MoneyYuan100)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(tuple =>
                       {
                           MoneyInside = new Money(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7);
                           MoneyAmount = MoneyInside.Amount;
                       });

        // Stream events subscription.
        this.WhenActivated(disposable =>
                           {
                               // When the cluster client changes, subscribe to the machine info stream.
                               var machineStreamObs = this.WhenAnyValue(vm => vm.Id, vm => vm.ClusterClient)
                                                          .Where(tuple => tuple.Item1 != Guid.Empty && tuple.Item2 != null)
                                                          .SelectMany(tuple => tuple.Item2!.GetSubscriberStreamWithGuidKey<MachineEvent>(Constants.StreamProviderName, Constants.MachinesNamespace, tuple.Item1, _lastSequenceToken))
                                                          .Publish()
                                                          .RefCount();
                               machineStreamObs.Where(tuple => tuple.Event is MachineInitializedEvent)
                                               .ObserveOn(RxApp.MainThreadScheduler)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineInitializedEvent)tuple.Event;
                                                              UpdateWith(machineEvent.MoneyInside);
                                                              _slotsCache.LoadWith(machineEvent.Slots, _snacks);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineDeletedEvent)
                                               .ObserveOn(RxApp.MainThreadScheduler)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineDeletedEvent)tuple.Event;
                                                              UpdateWith(machineEvent.MoneyInside);
                                                              IsDeleted = true;
                                                              _slotsCache.LoadWith(machineEvent.Slots, _snacks);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineUpdatedEvent)
                                               .ObserveOn(RxApp.MainThreadScheduler)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineUpdatedEvent)tuple.Event;
                                                              UpdateWith(machineEvent.MoneyInside);
                                                              _slotsCache.LoadWith(machineEvent.Slots, _snacks);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineSlotAddedEvent)
                                               .ObserveOn(RxApp.MainThreadScheduler)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineSlotAddedEvent)tuple.Event;
                                                              _slotsCache.AddOrUpdateWith(machineEvent.Slot, _snacks);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineSlotRemovedEvent)
                                               .ObserveOn(RxApp.MainThreadScheduler)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineSlotRemovedEvent)tuple.Event;
                                                              _slotsCache.Edit(updater => updater.Remove(machineEvent.Slot.Position));
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineMoneyLoadedEvent)
                                               .ObserveOn(RxApp.MainThreadScheduler)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineMoneyLoadedEvent)tuple.Event;
                                                              UpdateWith(machineEvent.MoneyInside);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineMoneyUnloadedEvent)
                                               .ObserveOn(RxApp.MainThreadScheduler)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineMoneyUnloadedEvent)tuple.Event;
                                                              UpdateWith(machineEvent.MoneyInside);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineMoneyInsertedEvent)
                                               .ObserveOn(RxApp.MainThreadScheduler)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineMoneyInsertedEvent)tuple.Event;
                                                              UpdateWith(machineEvent.MoneyInside);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineMoneyReturnedEvent)
                                               .ObserveOn(RxApp.MainThreadScheduler)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineMoneyReturnedEvent)tuple.Event;
                                                              UpdateWith(machineEvent.MoneyInside);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineSnacksLoadedEvent)
                                               .ObserveOn(RxApp.MainThreadScheduler)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineSnacksLoadedEvent)tuple.Event;
                                                              _slotsCache.AddOrUpdateWith(machineEvent.Slot, _snacks);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineSnacksUnloadedEvent)
                                               .ObserveOn(RxApp.MainThreadScheduler)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineSnacksUnloadedEvent)tuple.Event;
                                                              _slotsCache.AddOrUpdateWith(machineEvent.Slot, _snacks);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineSnackBoughtEvent)
                                               .ObserveOn(RxApp.MainThreadScheduler)
                                               .Subscribe(tuple =>
                                                          {
                                                              _lastSequenceToken = tuple.SequenceToken;
                                                              var machineEvent = (MachineSnackBoughtEvent)tuple.Event;
                                                              _slotsCache.AddOrUpdateWith(machineEvent.Slot, _snacks);
                                                          })
                                               .DisposeWith(disposable);
                               machineStreamObs.Where(tuple => tuple.Event is MachineErrorEvent)
                                               .ObserveOn(RxApp.MainThreadScheduler)
                                               .Subscribe(tuple =>
                                                          {
                                                              var errorEvent = (MachineErrorEvent)tuple.Event;
                                                              ErrorInfo = $"{errorEvent.Code}:{string.Join("\n", errorEvent.Reasons)}";
                                                          })
                                               .DisposeWith(disposable);
                           });

        // Create the commands.
        AddSlotCommand = ReactiveCommand.CreateFromTask(AddSlotAsync, CanAddSlot);
        RemoveSlotCommand = ReactiveCommand.CreateFromTask(RemoveSlotAsync, CanRemoveSlot);
        SaveMachineCommand = ReactiveCommand.CreateFromTask(SaveMachineAsync, CanSaveMachine);

        // Load the machine.
        UpdateWith(machine);
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

    private readonly ReadOnlyObservableCollection<SlotEditViewModel> _slots;
    public ReadOnlyObservableCollection<SlotEditViewModel> Slots => _slots;

    private int _slotCount;
    public int SlotCount
    {
        get => _slotCount;
        set => this.RaiseAndSetIfChanged(ref _slotCount, value);
    }

    private readonly ReadOnlyObservableCollection<SnackViewModel> _snacks;

    #endregion

    #region Commands

    /// <summary>
    ///     Gets the command that adds a new slot.
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddSlotCommand { get; }

    private IObservable<bool> CanAddSlot =>
        this.WhenAnyValue(vm => vm._snacks)
            .Select(_ => _snacks.IsNotNullOrEmpty());

    /// <summary>
    ///     Adds a new slot.
    /// </summary>
    private async Task AddSlotAsync()
    {
        bool retry;
        do
        {
            var result = Result.Ok()
                               .Ensure(_snacks.Count > 0, "No snacks available.")
                               .TapTry(() =>
                                       {
                                           var position = _slotsCache.Keys.Max() + 1;
                                           _slotsCache.Edit(updater => updater.AddOrUpdate(new SlotEditViewModel(new MachineSlot(Id, position), _snacks)));
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
        this.WhenAnyValue(vm => vm.Slots, vm => vm.SlotCount, vm => vm.IsDeleted, vm => vm.ClusterClient)
            .Select(tuple => tuple.Item1.IsNotNullOrEmpty()
                          && tuple.Item1.GroupBy(s => s.Position)
                                  .All(g => g.Count() == 1)
                          && tuple is { Item3: false, Item4: not null });

    private async Task SaveMachineAsync()
    {
        bool retry;
        do
        {
            IMachineRepoGrain grain = null!;
            Dictionary<int, SnackPile?> slots = null!;
            var result = await Result.Ok()
                                     .Ensure(Slots.IsNotNullOrEmpty(), "No slots available.")
                                     .Ensure(Slots.GroupBy(s => s.Position)
                                                  .All(g => g.Count() == 1), "Duplicate slot positions.")
                                     .Ensure(ClusterClient != null, "No cluster client available.")
                                     .TapTry(() => slots = Slots.ToDictionary(slot => slot.Position, slot => slot.SnackPile))
                                     .TapTry(() => grain = ClusterClient!.GetGrain<IMachineRepoGrain>("Manager"))
                                     .BindTryIfAsync(Id == Guid.Empty, () => grain.CreateAsync(new MachineRepoCreateCommand(MoneyInside, slots, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
                                     .BindTryIfAsync<Machine>(Id != Guid.Empty, () => grain.UpdateAsync(new MachineRepoUpdateCommand(Id, MoneyInside, slots, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
                                     .TapTryAsync(UpdateWith);
            if (result.IsSuccess)
            {
                return;
            }
            var errorRecovery = await Interactions.Errors.Handle(result.Errors);
            retry = errorRecovery == ErrorRecoveryOption.Retry;
        }
        while (retry);
    }

    #endregion

    #region Load Machine

    public void UpdateWith(Machine machine)
    {
        Id = machine.Id;
        UpdateWith(machine.MoneyInside);
        IsDeleted = machine.IsDeleted;
        _slotsCache.Edit(updater => updater.Load(machine.Slots.Select(slot => new SlotEditViewModel(slot, _snacks))));
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

}