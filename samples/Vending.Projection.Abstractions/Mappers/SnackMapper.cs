using Vending.Projection.Abstractions.Entities;

namespace Vending.Projection.Abstractions.Mappers;

public static class SnackMapper
{
    /// <summary>
    ///     Maps a <see cref="Domain.Abstractions.States.Snack" /> to a <see cref="Snack" />
    /// </summary>
    /// <param name="snackInGrain">The <see cref="Domain.Abstractions.States.Snack" /> to map from.</param>
    /// <param name="snack">The <see cref="Snack" /> to map to. If null, a new instance will be created.</param>
    /// <returns>The mapped <see cref="Snack" />.</returns>
    public static Snack ToProjection(this Domain.Abstractions.States.Snack snackInGrain, Snack? snack = null)
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
