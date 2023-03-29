using Vending.Projection.Abstractions.Entities;

namespace Vending.Projection.Abstractions.Mappers;

public static class SnackMapper
{
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
