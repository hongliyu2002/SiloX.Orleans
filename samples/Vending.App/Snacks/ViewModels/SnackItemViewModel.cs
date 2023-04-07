using System;
using Fluxera.Utilities.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vending.Projection.Abstractions.Snacks;

namespace Vending.App.ViewModels;

public class SnackItemViewModel : ReactiveObject
{
    private const string DefaultUrl = "pack://application:,,,/Vending.App;component/Images/snackInfo.png";

    public SnackItemViewModel(SnackInfo snackInfo)
    {
        LoadWith(snackInfo);
    }

    [Reactive]
    public Guid Id { get; set; }

    [Reactive]
    public bool IsDeleted { get; set; }

    [Reactive]
    public string Name { get; set; } = string.Empty;

    [Reactive]
    public Uri PictureUrl { get; set; } = new(DefaultUrl);

    public void LoadWith(SnackInfo snackInfo)
    {
        Id = snackInfo.Id;
        IsDeleted = snackInfo.IsDeleted;
        Name = snackInfo.Name;
        PictureUrl = snackInfo.PictureUrl.IsNotNullOrEmpty() ? new Uri(snackInfo.PictureUrl!) : new Uri(DefaultUrl);
    }
}
