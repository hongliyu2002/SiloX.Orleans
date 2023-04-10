using System;
using System.Reactive.Linq;
using System.Windows;
using Fluxera.Guards;
using ReactiveUI;
using Vending.Domain.Abstractions.Machines;

namespace Vending.App.Machines;

public class MachineSlotEditViewModel : ReactiveObject
{
    public MachineSlotEditViewModel(MachineSlot slot)
    {
        Guard.Against.Null(slot, nameof(slot));
        // Recreate the SnackPile when any of the properties change.
        this.WhenAnyValue(vm => vm.SnackId, vm => vm.Quantity, vm => vm.Price)
            .Where(pile => pile is { Item1: not null, Item2: not null, Item3: not null })
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(yuan =>
                       {
                           SnackPile = new SnackPile(SnackId!.Value, Quantity!.Value, Price!.Value);
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