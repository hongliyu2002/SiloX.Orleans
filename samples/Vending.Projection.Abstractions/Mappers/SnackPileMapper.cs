using Vending.Projection.Abstractions.Entities;

namespace Vending.Projection.Abstractions.Mappers;

public static class SnackPileMapper
{
    public static SnackPile ToProjection(this Domain.Abstractions.States.SnackPile source, SnackPile? snackPile = null)
    {
        snackPile ??= new SnackPile();
        snackPile.SnackId = source.SnackId;
        snackPile.Quantity = source.Quantity;
        snackPile.Price = source.Price;
        snackPile.TotalPrice = source.TotalPrice;
        return snackPile;
    }

    public static Domain.Abstractions.States.SnackPile ToDomain(this SnackPile source)
    {
        return new Domain.Abstractions.States.SnackPile(source.SnackId, source.Quantity, source.Price);
    }
}
