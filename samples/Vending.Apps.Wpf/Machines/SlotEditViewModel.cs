using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Fluxera.Guards;
using ReactiveUI;
using Vending.App.Wpf.Snacks;
using Vending.Domain.Abstractions.Machines;

namespace Vending.App.Wpf.Machines;

public class SlotEditViewModel : ReactiveObject
{
    public SlotEditViewModel(MachineSlot slot, ReadOnlyObservableCollection<SnackViewModel> snacks)
    {
        Guard.Against.Null(slot, nameof(slot));
        Snacks = Guard.Against.Null(snacks, nameof(snacks));

        // Set the current snack when the snack id changes.
        this.WhenAnyValue(vm => vm.SnackId)
            .Where(snackId => snackId != null && snackId.Value != Guid.Empty)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(snackId =>
                       {
                           var snack = CurrentSnack;
                           if (snack == null || snack.Id != snackId!.Value)
                           {
                               CurrentSnack = Snacks.FirstOrDefault(s => s.Id == snackId);
                           }
                       });

        // Recreate the snack pile when any of the properties change.
        this.WhenAnyValue(vm => vm.CurrentSnack, vm => vm.Quantity, vm => vm.Price)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(tuple =>
                       {
                           var snack = tuple.Item1;
                           var quantity = tuple.Item2;
                           var price = tuple.Item3;
                           if (snack == null)
                           {
                               SnackPile = null;
                               Amount = null;
                           }
                           else if (quantity == null)
                           {
                               Quantity = 1;
                           }
                           else if (price == null)
                           {
                               Price = 0;
                           }
                           else
                           {
                               SnackPile = new SnackPile(snack.Id, quantity.Value, price.Value);
                               Amount = SnackPile.Amount;
                           }
                       });

        // Load the slot.
        LoadSlot(slot);
    }

    #region Properties

    private int _position;
    public int Position
    {
        get => _position;
        set => this.RaiseAndSetIfChanged(ref _position, value);
    }

    private SnackPile? _snackPile;
    public SnackPile? SnackPile
    {
        get => _snackPile;
        set => this.RaiseAndSetIfChanged(ref _snackPile, value);
    }

    private Guid? _snackId;
    public Guid? SnackId
    {
        get => _snackId;
        set => this.RaiseAndSetIfChanged(ref _snackId, value);
    }

    private int? _quantity;
    public int? Quantity
    {
        get => _quantity;
        set => this.RaiseAndSetIfChanged(ref _quantity, value);
    }

    private decimal? _price;
    public decimal? Price
    {
        get => _price;
        set => this.RaiseAndSetIfChanged(ref _price, value);
    }

    private decimal? _amount;
    public decimal? Amount
    {
        get => _amount;
        set => this.RaiseAndSetIfChanged(ref _amount, value);
    }

    private SnackViewModel? _currentSnack;
    public SnackViewModel? CurrentSnack
    {
        get => _currentSnack;
        set => this.RaiseAndSetIfChanged(ref _currentSnack, value);
    }

    public ReadOnlyObservableCollection<SnackViewModel> Snacks { get; }

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