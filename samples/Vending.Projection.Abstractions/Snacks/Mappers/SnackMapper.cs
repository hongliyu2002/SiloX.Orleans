namespace Vending.Projection.Abstractions.Snacks;

public static class SnackMapper
{
    /// <summary>
    ///     Maps a <see cref="Domain.Abstractions.Snacks.Snack" /> to a <see cref="Snack" />
    /// </summary>
    /// <param name="snackInGrain">The <see cref="Domain.Abstractions.Snacks.Snack" /> to map from.</param>
    /// <param name="snack">The <see cref="Snack" /> to map to. If null, a new instance will be created.</param>
    /// <returns>The mapped <see cref="Snack" />.</returns>
    public static Snack ToProjection(this Domain.Abstractions.Snacks.Snack snackInGrain, Snack? snack = null)
    {
        snack ??= new Snack();
        snack.Id = snackInGrain.Id;
        snack.Name = snackInGrain.Name;
        snack.PictureUrl = snackInGrain.PictureUrl;
        snack.CreatedAt = snackInGrain.CreatedAt;
        snack.LastModifiedAt = snackInGrain.LastModifiedAt;
        snack.DeletedAt = snackInGrain.DeletedAt;
        snack.CreatedBy = snackInGrain.CreatedBy;
        snack.LastModifiedBy = snackInGrain.LastModifiedBy;
        snack.DeletedBy = snackInGrain.DeletedBy;
        snack.IsDeleted = snackInGrain.IsDeleted;
        return snack;
    }
}
