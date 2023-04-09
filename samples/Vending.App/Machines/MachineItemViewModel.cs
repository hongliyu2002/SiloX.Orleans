using System;
using Fluxera.Guards;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vending.Projection.Abstractions.Machines;

namespace Vending.App.Machines;

public class MachineItemViewModel : ReactiveObject
{
    public MachineItemViewModel(MachineInfo machineInfo)
    {
        machineInfo = Guard.Against.Null(machineInfo, nameof(machineInfo));
        Id = machineInfo.Id;
        MoneyInside = machineInfo.MoneyInside;
        AmountInTransaction = machineInfo.AmountInTransaction;
        SlotsCount = machineInfo.SlotsCount;
        SnackCount = machineInfo.SnackCount;
        SnackQuantity = machineInfo.SnackQuantity;
        SnackAmount = machineInfo.SnackAmount;
        BoughtCount = machineInfo.BoughtCount;
        BoughtAmount = machineInfo.BoughtAmount;
        IsDeleted = machineInfo.IsDeleted;
    }

    [Reactive]
    public Guid Id { get; set; }

    [Reactive]
    public MoneyInfo MoneyInside { get; set; }

    [Reactive]
    public decimal AmountInTransaction { get; set; }

    [Reactive]
    public int SlotsCount { get; set; }

    [Reactive]
    public int SnackCount { get; set; }

    [Reactive]
    public int SnackQuantity { get; set; }

    [Reactive]
    public decimal SnackAmount { get; set; }

    [Reactive]
    public int BoughtCount { get; set; }

    [Reactive]
    public decimal BoughtAmount { get; set; }

    [Reactive]
    public bool IsDeleted { get; set; }
}