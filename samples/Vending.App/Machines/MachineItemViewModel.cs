using System;
using Fluxera.Guards;
using ReactiveUI;
using Vending.Projection.Abstractions.Machines;

namespace Vending.App.Machines;

public class MachineItemViewModel : ReactiveObject
{
    public MachineItemViewModel(MachineInfo machineInfo)
    {
        Guard.Against.Null(machineInfo, nameof(machineInfo));
        LoadMachine(machineInfo);
    }

    private Guid _id;

    public Guid Id
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    private MoneyInfo _moneyInside = new();

    public MoneyInfo MoneyInside
    {
        get => _moneyInside;
        set => this.RaiseAndSetIfChanged(ref _moneyInside, value);
    }

    private decimal _amountInTransaction;

    public decimal AmountInTransaction
    {
        get => _amountInTransaction;
        set => this.RaiseAndSetIfChanged(ref _amountInTransaction, value);
    }

    private int _slotCount;

    public int SlotCount
    {
        get => _slotCount;
        set => this.RaiseAndSetIfChanged(ref _slotCount, value);
    }

    private int _snackCount;

    public int SnackCount
    {
        get => _snackCount;
        set => this.RaiseAndSetIfChanged(ref _snackCount, value);
    }

    private int _snackQuantity;

    public int SnackQuantity
    {
        get => _snackQuantity;
        set => this.RaiseAndSetIfChanged(ref _snackQuantity, value);
    }

    private decimal _snackAmount;

    public decimal SnackAmount
    {
        get => _snackAmount;
        set => this.RaiseAndSetIfChanged(ref _snackAmount, value);
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

    #region Load Machine

    private void LoadMachine(MachineInfo machineInfo)
    {
        Id = machineInfo.Id;
        MoneyInside = machineInfo.MoneyInside;
        AmountInTransaction = machineInfo.AmountInTransaction;
        SlotCount = machineInfo.SlotCount;
        SnackCount = machineInfo.SnackCount;
        SnackQuantity = machineInfo.SnackQuantity;
        SnackAmount = machineInfo.SnackAmount;
        BoughtCount = machineInfo.BoughtCount;
        BoughtAmount = machineInfo.BoughtAmount;
        IsDeleted = machineInfo.IsDeleted;
    }

    #endregion

}