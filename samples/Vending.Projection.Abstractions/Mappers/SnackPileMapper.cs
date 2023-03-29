using Vending.Projection.Abstractions.Entities;

namespace Vending.Projection.Abstractions.Mappers;

public static class SnackPileMapper
{
    /// <summary>
    ///     Maps a <see cref="Domain.Abstractions.States.SnackPile" /> to a <see cref="SnackPile" />
    /// </summary>
    /// <param name="snackPileInGrain">The <see cref="Domain.Abstractions.States.SnackPile" /> to map from.</param>
    /// <param name="getNamePicture">A function that returns the name and picture url of a snack.</param>
    /// <param name="snackPile">The <see cref="SnackPile" /> to map to. If null, a new instance will be created.</param>
    /// <returns>The mapped <see cref="SnackPile" />.</returns>
    public static async Task<SnackPile> ToProjection(this Domain.Abstractions.States.SnackPile snackPileInGrain, Func<Guid, Task<(string, string?)>> getNamePicture, SnackPile? snackPile = null)
    {
        snackPile ??= new SnackPile();
        snackPile.SnackId = snackPileInGrain.SnackId;
        snackPile.Quantity = snackPileInGrain.Quantity;
        snackPile.Price = snackPileInGrain.Price;
        snackPile.TotalAmount = snackPileInGrain.TotalAmount;
        await snackPile.UpdateSnackNameAndPictureUrlAsync(getNamePicture);
        return snackPile;
    }
}
