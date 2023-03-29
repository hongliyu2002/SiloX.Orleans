using Vending.Projection.Abstractions.Entities;

namespace Vending.Projection.Abstractions.Mappers;

public static class PurchaseMapper
{
    /// <summary>
    ///     Maps a <see cref="Domain.Abstractions.States.Purchase" /> to a <see cref="Purchase" />.
    /// </summary>
    /// <param name="purchaseInGrain">The <see cref="Domain.Abstractions.States.Purchase" /> to map. </param>
    /// <param name="getSnackNamePicture">A function that returns the name and picture url of the snack. </param>
    /// <param name="purchase">The <see cref="Purchase" /> to map to. </param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
    public static async Task<Purchase> ToProjection(this Domain.Abstractions.States.Purchase purchaseInGrain, Func<Guid, Task<(string, string?)>> getSnackNamePicture, Purchase? purchase = null)
    {
        purchase ??= new Purchase();
        purchase.MachineId = purchaseInGrain.MachineId;
        purchase.Position = purchaseInGrain.Position;
        purchase.SnackId = purchaseInGrain.SnackId;
        purchase.BoughtPrice = purchaseInGrain.BoughtPrice;
        purchase.BoughtAt = purchaseInGrain.BoughtAt;
        purchase.BoughtBy = purchaseInGrain.BoughtBy;
        await purchase.UpdateSnackNameAndPictureUrlAsync(getSnackNamePicture);
        return purchase;
    }
}
