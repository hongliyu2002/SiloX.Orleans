using System;
using Fluxera.Utilities.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vending.Domain.Abstractions.Snacks;

namespace Vending.App.ViewModels;

public class SnackEditViewModel : ReactiveObject
{
    private const string DefaultUrl = "pack://application:,,,/Vending.App;component/Images/snack.png";

    public SnackEditViewModel(Snack snack)
    {
        LoadWith(snack);
    }

    [Reactive]
    public Guid Id { get; set; }

    [Reactive]
    public bool IsDeleted { get; set; }

    [Reactive]
    public string Name { get; set; } = string.Empty;

    [Reactive]
    public Uri PictureUrl { get; set; } = new(DefaultUrl);

    public void LoadWith(Snack snack)
    {
        Id = snack.Id;
        IsDeleted = snack.IsDeleted;
        Name = snack.Name;
        PictureUrl = snack.PictureUrl.IsNotNullOrEmpty() ? new Uri(snack.PictureUrl!) : new Uri(DefaultUrl);
    }
}
