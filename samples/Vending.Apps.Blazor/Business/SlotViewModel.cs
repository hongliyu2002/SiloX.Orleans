using System.Reactive.Linq;
using Fluxera.Guards;
using ReactiveUI;
using Vending.Domain.Abstractions.Machines;

namespace Vending.Apps.Blazor.Business;

public class SlotViewModel : ReactiveObject
{

    #region Constructor

    public SlotViewModel(MachineSlot slot)
    {
        Guard.Against.Null(slot, nameof(slot));
        // Set the current snack when the snack id changes.
        this.WhenAnyValue(vm => vm.SnackId)
            .DistinctUntilChanged()
            .Subscribe(snackId =>
                       {
                           if (snackId == null || snackId.Value == Guid.Empty)
                           {
                               Snack = null;
                           }
                           else
                           {
                               var snack = Snack;
                               if (snack == null || snack.Id != snackId.Value)
                               {
                                   Snack = new SnackViewModel(snackId.Value);
                               }
                           }
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

    private SnackViewModel? _snack;
    public SnackViewModel? Snack
    {
        get => _snack;
        set => this.RaiseAndSetIfChanged(ref _snack, value);
    }

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
        }
        else
        {
            SnackId = null;
            Quantity = null;
            Price = null;
        }
    }

    #endregion

}