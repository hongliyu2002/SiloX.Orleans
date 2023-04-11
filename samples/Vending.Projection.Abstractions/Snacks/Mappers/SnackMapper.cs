using Vending.Domain.Abstractions.Snacks;

namespace Vending.Projection.Abstractions.Snacks;

public static class SnackMapper
{
    /// <summary>
    ///     Maps a <see cref="Snack" /> to a <see cref="SnackInfo" />
    /// </summary>
    /// <param name="snack">The <see cref="Snack" /> to map from.</param>
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

    /// <summary>
    ///     Maps a <see cref="SnackInfo" /> to a <see cref="Snack" />
    /// </summary>
    /// <param name="snackInfo">The <see cref="SnackInfo" /> to map from.</param>
    /// <param name="snack">The <see cref="Snack" /> to map to. If null, a new instance will be created.</param>
    /// <returns>The mapped <see cref="Snack" />.</returns>
    public static Snack ToDomain(this SnackInfo snackInfo, Snack? snack = null)
    {
        snack ??= new Snack();
        snack.Id = snackInfo.Id;
        snack.Name = snackInfo.Name;
        snack.PictureUrl = snackInfo.PictureUrl;
        snack.CreatedAt = snackInfo.CreatedAt;
        snack.CreatedBy = snackInfo.CreatedBy;
        snack.LastModifiedAt = snackInfo.LastModifiedAt;
        snack.LastModifiedBy = snackInfo.LastModifiedBy;
        snack.DeletedBy = snackInfo.DeletedBy;
        snack.DeletedAt = snackInfo.DeletedAt;
        snack.IsDeleted = snackInfo.IsDeleted;
        return snack;
    }
}