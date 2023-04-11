using Vending.Domain.Abstractions.Purchases;

namespace Vending.Projection.Abstractions.Purchases;

public static class PurchaseMapper
{
    /// <summary>
    ///     Maps a <see cref="Purchase" /> to a <see cref="PurchaseInfo" />.
    /// </summary>
    /// <param name="purchase">The <see cref="Purchase" /> to map. </param>
    /// <param name="getSnackNamePicture">A function that returns the name and picture url of the snack. </param>
    /// <param name="purchaseInfo">The <see cref="PurchaseInfo" /> to map to. </param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
    public static async Task<PurchaseInfo> ToProjection(this Purchase purchase, Func<Guid, Task<(string, string?)>> getSnackNamePicture, PurchaseInfo? purchaseInfo = null)
    {
        purchaseInfo ??= new PurchaseInfo();
        purchaseInfo.Id = purchase.Id;
        purchaseInfo.MachineId = purchase.MachineId;
        purchaseInfo.Position = purchase.Position;
        purchaseInfo.SnackId = purchase.SnackId;
        purchaseInfo.BoughtPrice = purchase.BoughtPrice;
        purchaseInfo.BoughtAt = purchase.BoughtAt;
        purchaseInfo.BoughtBy = purchase.BoughtBy;
        await purchaseInfo.UpdateSnackNameAndPictureUrlAsync(getSnackNamePicture);
        return purchaseInfo;
    }

    /// <summary>
    ///     Maps a <see cref="PurchaseInfo" /> to a <see cref="Purchase" />.
    /// </summary>
    /// <param name="purchaseInfo">The <see cref="PurchaseInfo" /> to map. </param>
    /// <param name="purchase">The <see cref="Purchase" /> to map to. </param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
    public static Purchase ToDomain(this PurchaseInfo purchaseInfo, Purchase? purchase = null)
    {
        purchase ??= new Purchase();
        purchase.Id = purchaseInfo.Id;
        purchase.MachineId = purchaseInfo.MachineId;
        purchase.Position = purchaseInfo.Position;
        purchase.SnackId = purchaseInfo.SnackId;
        purchase.BoughtPrice = purchaseInfo.BoughtPrice;
        purchase.BoughtAt = purchaseInfo.BoughtAt;
        purchase.BoughtBy = purchaseInfo.BoughtBy;
        return purchase;
    }
}