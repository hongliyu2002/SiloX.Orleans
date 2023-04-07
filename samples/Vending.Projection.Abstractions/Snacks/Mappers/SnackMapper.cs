using Vending.Domain.Abstractions.Snacks;

namespace Vending.Projection.Abstractions.Snacks;

public static class SnackMapper
{
    /// <summary>
    ///     Maps a <see cref="Domain.Abstractions.Snacks.Snack" /> to a <see cref="SnackInfo" />
    /// </summary>
    /// <param name="snack">The <see cref="Domain.Abstractions.Snacks.Snack" /> to map from.</param>
    /// <param name="snackInfo">The <see cref="SnackInfo" /> to map to. If null, a new instance will be created.</param>
    /// <returns>The mapped <see cref="SnackInfo" />.</returns>
    public static SnackInfo ToProjection(this Snack snack, SnackInfo? snackInfo = null)
    {
        snackInfo ??= new SnackInfo();
        snackInfo.Id = snack.Id;
        snackInfo.Name = snack.Name;
        snackInfo.PictureUrl = snack.PictureUrl;
        snackInfo.CreatedAt = snack.CreatedAt;
        snackInfo.CreatedBy = snack.CreatedBy;
        snackInfo.LastModifiedAt = snack.LastModifiedAt;
        snackInfo.LastModifiedBy = snack.LastModifiedBy;
        snackInfo.DeletedBy = snack.DeletedBy;
        snackInfo.DeletedAt = snack.DeletedAt;
        snackInfo.IsDeleted = snack.IsDeleted;
        return snackInfo;
    }
}
