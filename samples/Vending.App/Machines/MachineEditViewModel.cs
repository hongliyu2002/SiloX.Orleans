using System;
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
using Orleans;
using Orleans.FluentResults;
using Orleans.Runtime;
using Orleans.Streams;
using ReactiveUI;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;

namespace Vending.App.Machines;

public class MachineEditViewModel : ReactiveObject, IActivatableViewModel, IOrleansObject
{
    private readonly SourceCache<MachineSlotEditViewModel, int> _slotsCache;
    private readonly ReadOnlyObservableCollection<MachineSlotEditViewModel> _slots;

    private StreamSubscriptionHandle<MachineEvent>? _subscription;
    private StreamSequenceToken? _lastSequenceToken;

    public MachineEditViewModel(Machine machine, IClusterClient clusterClient)
    {
        Guard.Against.Null(machine, nameof(machine));
        ClusterClient = Guard.Against.Null(clusterClient, nameof(clusterClient));
        // Create the cache for the slots.
        _slotsCache = new SourceCache<MachineSlotEditViewModel, int>(slot => slot.Position);
        _slotsCache.Connect()
                   .Sort(SortExpressionComparer<MachineSlotEditViewModel>.Ascending(slot => slot.Position))
                   .ObserveOn(RxApp.MainThreadScheduler)
                   .Bind(out _slots)
                   .Subscribe();
        // Recreate the money inside when any of the money properties change.
        this.WhenAnyValue(vm => vm.MoneyYuan1, vm => vm.MoneyYuan2, vm => vm.MoneyYuan5, vm => vm.MoneyYuan10, vm => vm.MoneyYuan20, vm => vm.MoneyYuan50, vm => vm.MoneyYuan100)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(yuan =>
                       {
                           MoneyInside = new Money(yuan.Item1, yuan.Item2, yuan.Item3, yuan.Item4, yuan.Item5, yuan.Item6, yuan.Item7);
                           MoneyAmount = MoneyInside.Amount;
                       });
        this.WhenActivated(disposable =>
                           {
                               // When the cluster client changes, subscribe to the machine info stream.
                               this.WhenAnyValue(vm => vm.Id, vm => vm.ClusterClient)
                                   .Where(idClient => idClient.Item1 != Guid.Empty && idClient.Item2 != null)
                                   .Select(idClient => (idClient.Item1, idClient.Item2!.GetStreamProvider(Constants.StreamProviderName)))
                                   .Select(idProvider => idProvider.Item2.GetStream<MachineEvent>(StreamId.Create(Constants.MachinesNamespace, idProvider.Item1)))
                                   .SelectMany(stream => stream.SubscribeAsync(HandleEventAsync, HandleErrorAsync, HandleCompletedAsync, _lastSequenceToken))
                                   .ObserveOn(RxApp.MainThreadScheduler)
                                   .Subscribe(HandleSubscriptionAsync)
                                   .DisposeWith(disposable);
                               Disposable.Create(HandleSubscriptionDisposeAsync)
                                         .DisposeWith(disposable);
                           });
        // Create the commands.
        SaveMachineCommand = ReactiveCommand.CreateFromTask(SaveMachineAsync, CanSaveMachine);
        // Load the machine.
        LoadMachine(machine);
    }

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
        switch (domainEvent)
        {
            case MachineInitializedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineDeletedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineUpdatedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineSlotAddedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineSlotRemovedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineMoneyLoadedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineMoneyUnloadedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineMoneyInsertedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineMoneyReturnedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineSnacksLoadedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineSnacksUnloadedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineSnackBoughtEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineErrorEvent machineEvent:
                return ApplyErrorEventAsync(machineEvent);
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

    private Task ApplyEventAsync(MachineInitializedEvent machineEvent)
    {
        if (machineEvent.MachineId == Id)
        {
            MoneyYuan1 = machineEvent.MoneyInside.Yuan1;
            MoneyYuan2 = machineEvent.MoneyInside.Yuan2;
            MoneyYuan5 = machineEvent.MoneyInside.Yuan5;
            MoneyYuan10 = machineEvent.MoneyInside.Yuan10;
            MoneyYuan20 = machineEvent.MoneyInside.Yuan20;
            MoneyYuan50 = machineEvent.MoneyInside.Yuan50;
            MoneyYuan100 = machineEvent.MoneyInside.Yuan100;
            MoneyAmount = machineEvent.MoneyInside.Amount;
            _slotsCache.Edit(updater => updater.Load(machineEvent.Slots.Select(slot => new MachineSlotEditViewModel(slot))));
        }
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineDeletedEvent machineEvent)
    {
        if (machineEvent.MachineId == Id)
        {
            MoneyYuan1 = machineEvent.MoneyInside.Yuan1;
            MoneyYuan2 = machineEvent.MoneyInside.Yuan2;
            MoneyYuan5 = machineEvent.MoneyInside.Yuan5;
            MoneyYuan10 = machineEvent.MoneyInside.Yuan10;
            MoneyYuan20 = machineEvent.MoneyInside.Yuan20;
            MoneyYuan50 = machineEvent.MoneyInside.Yuan50;
            MoneyYuan100 = machineEvent.MoneyInside.Yuan100;
            MoneyAmount = machineEvent.MoneyInside.Amount;
            _slotsCache.Edit(updater => updater.Load(machineEvent.Slots.Select(slot => new MachineSlotEditViewModel(slot))));
            IsDeleted = true;
        }
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineUpdatedEvent machineEvent)
    {
        if (machineEvent.MachineId == Id)
        {
            MoneyYuan1 = machineEvent.MoneyInside.Yuan1;
            MoneyYuan2 = machineEvent.MoneyInside.Yuan2;
            MoneyYuan5 = machineEvent.MoneyInside.Yuan5;
            MoneyYuan10 = machineEvent.MoneyInside.Yuan10;
            MoneyYuan20 = machineEvent.MoneyInside.Yuan20;
            MoneyYuan50 = machineEvent.MoneyInside.Yuan50;
            MoneyYuan100 = machineEvent.MoneyInside.Yuan100;
            MoneyAmount = machineEvent.MoneyInside.Amount;
            _slotsCache.Edit(updater => updater.Load(machineEvent.Slots.Select(slot => new MachineSlotEditViewModel(slot))));
        }
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineSlotAddedEvent machineEvent)
    {
        if (machineEvent.MachineId == Id)
        {
            _slotsCache.Edit(updater => updater.AddOrUpdate(new MachineSlotEditViewModel(machineEvent.Slot)));
        }
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineSlotRemovedEvent machineEvent)
    {
        if (machineEvent.MachineId == Id)
        {
            _slotsCache.Edit(updater => updater.Remove(machineEvent.Position));
        }
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineMoneyLoadedEvent machineEvent)
    {
        if (machineEvent.MachineId == Id)
        {
            MoneyYuan1 = machineEvent.MoneyInside.Yuan1;
            MoneyYuan2 = machineEvent.MoneyInside.Yuan2;
            MoneyYuan5 = machineEvent.MoneyInside.Yuan5;
            MoneyYuan10 = machineEvent.MoneyInside.Yuan10;
            MoneyYuan20 = machineEvent.MoneyInside.Yuan20;
            MoneyYuan50 = machineEvent.MoneyInside.Yuan50;
            MoneyYuan100 = machineEvent.MoneyInside.Yuan100;
            MoneyAmount = machineEvent.MoneyInside.Amount;
        }
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineMoneyUnloadedEvent machineEvent)
    {
        if (machineEvent.MachineId == Id)
        {
            MoneyYuan1 = machineEvent.MoneyInside.Yuan1;
            MoneyYuan2 = machineEvent.MoneyInside.Yuan2;
            MoneyYuan5 = machineEvent.MoneyInside.Yuan5;
            MoneyYuan10 = machineEvent.MoneyInside.Yuan10;
            MoneyYuan20 = machineEvent.MoneyInside.Yuan20;
            MoneyYuan50 = machineEvent.MoneyInside.Yuan50;
            MoneyYuan100 = machineEvent.MoneyInside.Yuan100;
            MoneyAmount = machineEvent.MoneyInside.Amount;
        }
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineMoneyInsertedEvent machineEvent)
    {
        if (machineEvent.MachineId == Id)
        {
            MoneyYuan1 = machineEvent.MoneyInside.Yuan1;
            MoneyYuan2 = machineEvent.MoneyInside.Yuan2;
            MoneyYuan5 = machineEvent.MoneyInside.Yuan5;
            MoneyYuan10 = machineEvent.MoneyInside.Yuan10;
            MoneyYuan20 = machineEvent.MoneyInside.Yuan20;
            MoneyYuan50 = machineEvent.MoneyInside.Yuan50;
            MoneyYuan100 = machineEvent.MoneyInside.Yuan100;
            MoneyAmount = machineEvent.MoneyInside.Amount;
        }
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineMoneyReturnedEvent machineEvent)
    {
        if (machineEvent.MachineId == Id)
        {
            MoneyYuan1 = machineEvent.MoneyInside.Yuan1;
            MoneyYuan2 = machineEvent.MoneyInside.Yuan2;
            MoneyYuan5 = machineEvent.MoneyInside.Yuan5;
            MoneyYuan10 = machineEvent.MoneyInside.Yuan10;
            MoneyYuan20 = machineEvent.MoneyInside.Yuan20;
            MoneyYuan50 = machineEvent.MoneyInside.Yuan50;
            MoneyYuan100 = machineEvent.MoneyInside.Yuan100;
            MoneyAmount = machineEvent.MoneyInside.Amount;
        }
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineSnacksLoadedEvent machineEvent)
    {
        if (machineEvent.MachineId == Id)
        {
            _slotsCache.Edit(updater => updater.AddOrUpdate(new MachineSlotEditViewModel(machineEvent.Slot)));
        }
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineSnacksUnloadedEvent machineEvent)
    {
        if (machineEvent.MachineId == Id)
        {
            _slotsCache.Edit(updater => updater.AddOrUpdate(new MachineSlotEditViewModel(machineEvent.Slot)));
        }
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineSnackBoughtEvent machineEvent)
    {
        if (machineEvent.MachineId == Id)
        {
            _slotsCache.Edit(updater => updater.AddOrUpdate(new MachineSlotEditViewModel(machineEvent.Slot)));
        }
        return Task.CompletedTask;
    }

    private Task ApplyErrorEventAsync(MachineErrorEvent errorEvent)
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

    private Guid _id;

    public Guid Id
    {
        get => _id;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _id, value);
                               });
        }
    }

    private Money _moneyInside = Money.Zero;

    public Money MoneyInside
    {
        get => _moneyInside;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _moneyInside, value);
                               });
        }
    }

    private int _moneyYuan1;

    public int MoneyYuan1
    {
        get => _moneyYuan1;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _moneyYuan1, value);
                               });
        }
    }

    private int _moneyYuan2;

    public int MoneyYuan2
    {
        get => _moneyYuan2;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _moneyYuan2, value);
                               });
        }
    }
    private int _moneyYuan5;

    public int MoneyYuan5
    {
        get => _moneyYuan5;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _moneyYuan5, value);
                               });
        }
    }
    private int _moneyYuan10;

    public int MoneyYuan10
    {
        get => _moneyYuan10;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _moneyYuan10, value);
                               });
        }
    }

    private int _moneyYuan20;

    public int MoneyYuan20
    {
        get => _moneyYuan20;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _moneyYuan20, value);
                               });
        }
    }
    private int _moneyYuan50;

    public int MoneyYuan50
    {
        get => _moneyYuan50;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _moneyYuan50, value);
                               });
        }
    }

    private int _moneyYuan100;

    public int MoneyYuan100
    {
        get => _moneyYuan100;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _moneyYuan100, value);
                               });
        }
    }

    private decimal _moneyAmount;

    public decimal MoneyAmount
    {
        get => _moneyAmount;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _moneyAmount, value);
                               });
        }
    }

    private bool _isDeleted;

    public bool IsDeleted
    {
        get => _isDeleted;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _isDeleted, value);
                               });
        }
    }

    public ReadOnlyObservableCollection<MachineSlotEditViewModel> Slots => _slots;

    #endregion

    #region Commands

    /// <summary>
    ///     Gets the command to save the machine.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SaveMachineCommand { get; }

    private IObservable<bool> CanSaveMachine =>
        this.WhenAnyValue(vm => vm.IsDeleted, vm => vm.ClusterClient)
            .Select(deletedClient => deletedClient is { Item1: false, Item2: not null });

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
        _slotsCache.Edit(updater => updater.Load(machine.Slots.Select(slot => new MachineSlotEditViewModel(slot))));
    }

    #endregion

}