using System;
using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vending.Projection.Abstractions.Snacks;

namespace Vending.App.Snacks;

public class SnackItemViewModel : ReactiveObject
{
    private const string DefaultUrl = "pack://application:,,,/Vending.App;component/Images/snack.png";

    public SnackItemViewModel(SnackInfo snackInfo)
    {
        snackInfo = Guard.Against.Null(snackInfo, nameof(snackInfo));
        Id = snackInfo.Id;
        Name = snackInfo.Name;
        PictureUrl = snackInfo.PictureUrl.IsNotNullOrEmpty() ? new Uri(snackInfo.PictureUrl!) : new Uri(DefaultUrl);
        MachineCount = snackInfo.MachineCount;
        TotalQuantity = snackInfo.TotalQuantity;
        TotalAmount = snackInfo.TotalAmount;
        BoughtCount = snackInfo.BoughtCount;
        BoughtAmount = snackInfo.BoughtAmount;
        IsDeleted = snackInfo.IsDeleted;
    }

    [Reactive]
    public Guid Id { get; set; }

    [Reactive]
    public string Name { get; set; }

    [Reactive]
    public Uri PictureUrl { get; set; }

    [Reactive]
    public int MachineCount { get; set; }

    [Reactive]
    public int TotalQuantity { get; set; }

    [Reactive]
    public decimal TotalAmount { get; set; }

    [Reactive]
    public int BoughtCount { get; set; }

    [Reactive]
    public decimal BoughtAmount { get; set; }

    [Reactive]
    public bool IsDeleted { get; set; }
}