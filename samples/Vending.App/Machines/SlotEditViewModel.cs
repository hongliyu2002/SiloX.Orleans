using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using DynamicData;
using DynamicData.Binding;
using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using ReactiveUI;
using Vending.App.Snacks;
using Vending.Domain.Abstractions.Machines;

namespace Vending.App.Machines;

public class SlotEditViewModel : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<SnackViewModel> _snacks;

    public SlotEditViewModel(MachineSlot slot, SourceCache<SnackViewModel, Guid> snacksCache)
    {
        Guard.Against.Null(slot, nameof(slot));
        Guard.Against.Null(snacksCache, nameof(snacksCache));
        // Get the cache for the snacks.
        snacksCache.Connect()
                   .Sort(SortExpressionComparer<SnackViewModel>.Ascending(snack => snack.Name))
                   .ObserveOn(RxApp.MainThreadScheduler)
                   .Bind(out _snacks)
                   .Subscribe();
        // Set the current snack when the snack id changes.
        this.WhenAnyValue(vm => vm.SnackId, vm => vm.Snacks, vm => vm.CurrentSnack)
            .Where(pile => pile.Item1 != null && pile.Item1 != Guid.Empty && pile.Item2.IsNotNullOrEmpty() && (pile.Item3 == null || pile.Item3.Id != pile.Item1))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(pile =>
                       {
                           var snack = pile.Item2.FirstOrDefault(snack => snack.Id == pile.Item1);
                           CurrentSnack = snack;
                       });
        // Set the snack properties to null when the current snack is null.
        this.WhenAnyValue(vm => vm.CurrentSnack)
            .Where(snack => snack == null)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ =>
                       {
                           SnackPile = null;
                           SnackId = null;
                           Quantity = null;
                           Price = null;
                           Amount = null;
                       });
        // Set the snack properties when the current snack is not null.
        this.WhenAnyValue(vm => vm.CurrentSnack)
            .Where(snack => snack != null)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(snack =>
                       {
                           SnackId = snack!.Id;
                           Quantity ??= 1;
                           Price ??= 0;
                       });
        // Recreate the SnackPile when any of the properties change.
        this.WhenAnyValue(vm => vm.CurrentSnack, vm => vm.Quantity, vm => vm.Price)
            .Where(pile => pile is { Item1: not null, Item2: not null, Item3: not null })
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(pile =>
                       {
                           SnackPile = new SnackPile(pile.Item1!.Id, pile.Item2!.Value, pile.Item3!.Value);
                           Amount = SnackPile.Amount;
                       });
        // Load the slot.
        LoadSlot(slot);
    }

    #region Properties

    private int _position;
    public int Position
    {
        get => _position;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _position, value);
                               });
        }
    }

    private SnackPile? _snackPile;
    public SnackPile? SnackPile
    {
        get => _snackPile;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _snackPile, value);
                               });
        }
    }

    private Guid? _snackId;
    public Guid? SnackId
    {
        get => _snackId;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _snackId, value);
                               });
        }
    }

    private int? _quantity;
    public int? Quantity
    {
        get => _quantity;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _quantity, value);
                               });
        }
    }

    private decimal? _price;
    public decimal? Price
    {
        get => _price;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _price, value);
                               });
        }
    }

    private decimal? _amount;
    public decimal? Amount
    {
        get => _amount;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _amount, value);
                               });
        }
    }
    
    private SnackViewModel? _currentSnack;
    public SnackViewModel? CurrentSnack
    {
        get => _currentSnack;
        set => this.RaiseAndSetIfChanged(ref _currentSnack, value);
    }

    public ReadOnlyObservableCollection<SnackViewModel> Snacks => _snacks;

    #endregion

    #region Load Slot

    private void LoadSlot(MachineSlot slot)
    {
        Position = slot.Position;
        if (slot.SnackPile != null)
        {
            SnackId = slot.SnackPile.SnackId;
            Quantity = slot.SnackPile.Quantity;
            Price = slot.SnackPile.Price;
            Amount = slot.SnackPile.Amount;
        }
        else
        {
            SnackId = null;
            Quantity = null;
            Price = null;
            Amount = null;
        }
    }

    #endregion

}