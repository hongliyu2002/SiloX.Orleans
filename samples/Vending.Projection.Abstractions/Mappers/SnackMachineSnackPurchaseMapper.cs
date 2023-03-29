using Vending.Projection.Abstractions.Entities;

namespace Vending.Projection.Abstractions.Mappers;

public static class SnackMachineSnackPurchaseSnackPurchaseMapper
{
    /// <summary>
    ///     Maps a <see cref="Domain.Abstractions.States.SnackMachineSnackPurchase" /> to a <see cref="SnackMachineSnackPurchase" />.
    /// </summary>
    /// <param name="purchaseInGrain">The <see cref="Domain.Abstractions.States.SnackMachineSnackPurchase" /> to map. </param>
    /// <param name="getSnackNamePicture">A function that returns the name and picture url of the snack. </param>
    /// <param name="purchase">The <see cref="SnackMachineSnackPurchase" /> to map to. </param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
    public static async Task<SnackMachineSnackPurchase> ToProjection(this Domain.Abstractions.States.SnackMachineSnackPurchase purchaseInGrain, Func<Guid, Task<(string, string?)>> getSnackNamePicture, SnackMachineSnackPurchase? purchase = null)
    {
        purchase ??= new SnackMachineSnackPurchase();
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
