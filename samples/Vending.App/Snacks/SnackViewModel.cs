using System;
using Fluxera.Guards;
using ReactiveUI;
using Vending.Projection.Abstractions.Snacks;

namespace Vending.App.Snacks;

public class SnackViewModel : ReactiveObject
{
    public SnackViewModel(SnackInfo snackInfo)
    {
        Guard.Against.Null(snackInfo, nameof(snackInfo));
        // Load the snack info.
        LoadSnack(snackInfo);
    }

    #region Properties

    private Guid _id;

    public Guid Id
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    private string _name = string.Empty;

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    private string? _pictureUrl;

    public string? PictureUrl
    {
        get => _pictureUrl;
        set => this.RaiseAndSetIfChanged(ref _pictureUrl, value);
    }

    private int _machineCount;

    public int MachineCount
    {
        get => _machineCount;
        set => this.RaiseAndSetIfChanged(ref _machineCount, value);
    }

    private int _totalQuantity;

    public int TotalQuantity
    {
        get => _totalQuantity;
        set => this.RaiseAndSetIfChanged(ref _totalQuantity, value);
    }

    private decimal _totalAmount;

    public decimal TotalAmount
    {
        get => _totalAmount;
        set => this.RaiseAndSetIfChanged(ref _totalAmount, value);
    }

    private int _boughtCount;

    public int BoughtCount
    {
        get => _boughtCount;
        set => this.RaiseAndSetIfChanged(ref _boughtCount, value);
    }

    private decimal _boughtAmount;

    public decimal BoughtAmount
    {
        get => _boughtAmount;
        set => this.RaiseAndSetIfChanged(ref _boughtAmount, value);
    }

    private bool _isDeleted;

    public bool IsDeleted
    {
        get => _isDeleted;
        set => this.RaiseAndSetIfChanged(ref _isDeleted, value);
    }

    #endregion

    #region Load Snack

    private void LoadSnack(SnackInfo snackInfo)
    {
        Id = snackInfo.Id;
        Name = snackInfo.Name;
        PictureUrl = snackInfo.PictureUrl;
        MachineCount = snackInfo.MachineCount;
        TotalQuantity = snackInfo.TotalQuantity;
        TotalAmount = snackInfo.TotalAmount;
        BoughtCount = snackInfo.BoughtCount;
        BoughtAmount = snackInfo.BoughtAmount;
        IsDeleted = snackInfo.IsDeleted;
    }

    #endregion

}