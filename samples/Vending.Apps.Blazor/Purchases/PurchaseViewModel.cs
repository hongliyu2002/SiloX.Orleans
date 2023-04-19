using Fluxera.Guards;
using ReactiveUI;
using Vending.Projection.Abstractions.Purchases;

namespace Vending.Apps.Blazor.Purchases;

public class PurchaseViewModel : ReactiveObject
{
    public PurchaseViewModel(PurchaseInfo purchaseInfo)
    {
        Guard.Against.Null(purchaseInfo, nameof(purchaseInfo));
        // Load the purchase info.
        UpdateWith(purchaseInfo);
    }

    #region Properties

    private Guid _id;
    public Guid Id
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    private Guid _machineId;
    public Guid MachineId
    {
        get => _machineId;
        set => this.RaiseAndSetIfChanged(ref _machineId, value);
    }

    private int _position;
    public int Position
    {
        get => _position;
        set => this.RaiseAndSetIfChanged(ref _position, value);
    }

    private Guid _snackId;
    public Guid SnackId
    {
        get => _snackId;
        set => this.RaiseAndSetIfChanged(ref _snackId, value);
    }

    private string _snackName = string.Empty;
    public string SnackName
    {
        get => _snackName;
        set => this.RaiseAndSetIfChanged(ref _snackName, value);
    }

    private string? _snackPictureUrl;
    public string? SnackPictureUrl
    {
        get => _snackPictureUrl;
        set => this.RaiseAndSetIfChanged(ref _snackPictureUrl, value);
    }

    private decimal _boughtPrice;
    public decimal BoughtPrice
    {
        get => _boughtPrice;
        set => this.RaiseAndSetIfChanged(ref _boughtPrice, value);
    }

    private DateTimeOffset _boughtAt;
    public DateTimeOffset BoughtAt
    {
        get => _boughtAt;
        set => this.RaiseAndSetIfChanged(ref _boughtAt, value);
    }

    #endregion

    #region Load Purchase

    public void UpdateWith(PurchaseInfo purchaseInfo)
    {
        Id = purchaseInfo.Id;
        MachineId = purchaseInfo.MachineId;
        Position = purchaseInfo.Position;
        SnackId = purchaseInfo.SnackId;
        SnackName = purchaseInfo.SnackName;
        SnackPictureUrl = purchaseInfo.SnackPictureUrl;
        BoughtAt = purchaseInfo.BoughtAt ?? DateTimeOffset.MinValue;
        BoughtPrice = purchaseInfo.BoughtPrice;
    }

    #endregion

}