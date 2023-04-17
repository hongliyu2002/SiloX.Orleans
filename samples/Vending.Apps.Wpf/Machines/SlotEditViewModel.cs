using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Fluxera.Guards;
using ReactiveUI;
using Vending.Apps.Wpf.Snacks;
using Vending.Domain.Abstractions.Machines;

namespace Vending.Apps.Wpf.Machines;

public class SlotEditViewModel : ReactiveObject
{

    #region Constructor

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
        this.WhenAnyValue(vm => vm.CurrentSnack)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(snack =>
                       {
                           if (snack == null)
                           {
                               SnackPile = null;
                               Amount = null;
                           }
                           else
                           {
                               Quantity ??= 1;
                               Price ??= 1;
                           }
                       });
        this.WhenAnyValue(vm => vm.CurrentSnack, vm => vm.Quantity, vm => vm.Price)
            .Where(tuple => tuple is { Item1: not null, Item2: not null, Item3: not null })
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(tuple =>
                       {
                           SnackPile = new SnackPile(tuple.Item1!.Id, tuple.Item2!.Value, tuple.Item3!.Value);
                           Amount = SnackPile.Amount;
                       });
        // Load the slot.
        UpdateWith(slot);
    }

    #endregion

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

    public void UpdateWith(MachineSlot slot)
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