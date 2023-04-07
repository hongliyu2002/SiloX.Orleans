namespace Vending.Projection.Abstractions.Machines;

public static class SnackPileMapper
{
    /// <summary>
    ///     Maps a <see cref="Domain.Abstractions.Machines.SnackPile" /> to a <see cref="SnackPileInfo" />
    /// </summary>
    /// <param name="snackPile">The <see cref="Domain.Abstractions.Machines.SnackPile" /> to map from.</param>
    /// <param name="getNamePicture">A function that returns the name and picture url of a snack.</param>
    /// <param name="snackPileInfo">The <see cref="SnackPileInfo" /> to map to. If null, a new instance will be created.</param>
    /// <returns>The mapped <see cref="SnackPileInfo" />.</returns>
    public static async Task<SnackPileInfo> ToProjection(this Domain.Abstractions.Machines.SnackPile snackPile, Func<Guid, Task<(string, string?)>> getNamePicture, SnackPileInfo? snackPileInfo = null)
    {
        snackPileInfo ??= new SnackPileInfo();
        snackPileInfo.SnackId = snackPile.SnackId;
        snackPileInfo.Quantity = snackPile.Quantity;
        snackPileInfo.Price = snackPile.Price;
        snackPileInfo.TotalAmount = snackPile.Amount;
        await snackPileInfo.UpdateSnackNameAndPictureUrlAsync(getNamePicture);
        return snackPileInfo;
    }
}
