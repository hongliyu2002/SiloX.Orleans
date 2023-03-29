using Vending.Projection.Abstractions.Entities;

namespace Vending.Projection.Abstractions.Mappers;

public static class SnackPileMapper
{
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
