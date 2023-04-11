using Vending.Domain.Abstractions.Machines;

namespace Vending.Projection.Abstractions.Machines;

public static class SnackPileMapper
{
    /// <summary>
    ///     Maps a <see cref="SnackPile" /> to a <see cref="SnackPileInfo" />
    /// </summary>
    /// <param name="snackPile">The <see cref="SnackPile" /> to map from.</param>
    /// <param name="getNamePicture">A function that returns the name and picture url of a snack.</param>
    /// <param name="snackPileInfo">The <see cref="SnackPileInfo" /> to map to. If null, a new instance will be created.</param>
    /// <returns>The mapped <see cref="SnackPileInfo" />.</returns>
    public static async Task<SnackPileInfo> ToProjection(this SnackPile snackPile, Func<Guid, Task<(string, string?)>> getNamePicture, SnackPileInfo? snackPileInfo = null)
    {
        snackPileInfo ??= new SnackPileInfo();
        snackPileInfo.SnackId = snackPile.SnackId;
        snackPileInfo.Quantity = snackPile.Quantity;
        snackPileInfo.Price = snackPile.Price;
        snackPileInfo.Amount = snackPile.Amount;
        await snackPileInfo.UpdateSnackNameAndPictureUrlAsync(getNamePicture);
        return snackPileInfo;
    }

    /// <summary>
    ///     Maps a <see cref="SnackPileInfo" /> to a <see cref="SnackPile" />
    /// </summary>
    /// <param name="snackPileInfo">The <see cref="SnackPileInfo" /> to map from.</param>
    /// <param name="snackPile">The <see cref="SnackPile" /> to map to. If null, a new instance will be created.</param>
    /// <returns>The mapped <see cref="SnackPile" />.</returns>
    public static SnackPile ToDomain(this SnackPileInfo snackPileInfo, SnackPile? snackPile = null)
    {
        snackPile ??= new SnackPile(snackPileInfo.SnackId, snackPileInfo.Quantity, snackPileInfo.Price);
        return snackPile;
    }
}