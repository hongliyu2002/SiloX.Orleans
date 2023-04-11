using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Fluxera.Guards;
using ReactiveUI;
using Vending.App.Snacks;
using Vending.Domain.Abstractions.Machines;

namespace Vending.App.Machines;

public class SlotEditViewModel : ReactiveObject
{
    public SlotEditViewModel(MachineSlot slot, ReadOnlyObservableCollection<SnackViewModel> snacks)
    {
        Guard.Against.Null(slot, nameof(slot));
        Snacks = Guard.Against.Null(snacks, nameof(snacks));

        // Set the current snack when the snack id changes.
        this.WhenAnyValue(vm => vm.SnackId, vm => vm.CurrentSnack)
            .Where(_ => SnackId != null && SnackId != Guid.Empty && (CurrentSnack == null || CurrentSnack.Id != SnackId))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => CurrentSnack = Snacks.FirstOrDefault(snack => snack.Id == SnackId));

        // Set the snack properties to null when the current snack is null.
        this.WhenAnyValue(vm => vm.CurrentSnack)
            .Where(_ => CurrentSnack == null)
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
            .Where(_ => CurrentSnack != null)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(snack =>
                       {
                           SnackId = snack!.Id;
                           Quantity ??= 1;
                           Price ??= 0;
                       });

        // Recreate the snack pile when any of the properties change.
        this.WhenAnyValue(vm => vm.CurrentSnack, vm => vm.Quantity, vm => vm.Price)
            .Where(_ => CurrentSnack != null && Quantity != null && Price != null)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ =>
                       {
                           SnackPile = new SnackPile(CurrentSnack!.Id, Quantity!.Value, Price!.Value);
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