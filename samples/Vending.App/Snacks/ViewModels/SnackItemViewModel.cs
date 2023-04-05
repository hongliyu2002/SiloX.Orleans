using System;
using Fluxera.Utilities.Extensions;
using ReactiveUI;
using Vending.Projection.Abstractions.Snacks;

namespace Vending.App.ViewModels;

public class SnackItemViewModel : ReactiveObject
{
    private readonly Snack _snack;
    private readonly Uri _defaultUrl;

    /// <inheritdoc />
    public SnackItemViewModel(Snack snack)
    {
        _snack = snack;
        _defaultUrl = new Uri("https://git.io/fAlfh");
    }

    public string Name => _snack.Name;

    public Uri PictureUrl => _snack.PictureUrl.IsNullOrWhiteSpace() ? _defaultUrl : new Uri(_snack.PictureUrl!);
}
