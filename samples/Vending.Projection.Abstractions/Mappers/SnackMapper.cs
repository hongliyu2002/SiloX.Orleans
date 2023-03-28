using Vending.Projection.Abstractions.Entities;

namespace Vending.Projection.Abstractions.Mappers;

public static class SnackMapper
{
    public static Snack ToProjection(this Domain.Abstractions.States.Snack source, Snack? snack = null)
    {
        snack ??= new Snack();
        snack.Id = source.Id;
        snack.Name = source.Name;
        snack.PictureUrl = source.PictureUrl;
        snack.CreatedAt = source.CreatedAt;
        snack.LastModifiedAt = source.LastModifiedAt;
        snack.DeletedAt = source.DeletedAt;
        snack.CreatedBy = source.CreatedBy;
        snack.LastModifiedBy = source.LastModifiedBy;
        snack.DeletedBy = source.DeletedBy;
        snack.IsDeleted = source.IsDeleted;
        return snack;
    }

    public static Domain.Abstractions.States.Snack ToDomain(this Snack source, Domain.Abstractions.States.Snack? snack = null)
    {
        snack ??= new Domain.Abstractions.States.Snack();
        snack.Id = source.Id;
        snack.Name = source.Name;
        snack.PictureUrl = source.PictureUrl;
        snack.CreatedAt = source.CreatedAt;
        snack.LastModifiedAt = source.LastModifiedAt;
        snack.DeletedAt = source.DeletedAt;
        snack.CreatedBy = source.CreatedBy;
        snack.LastModifiedBy = source.LastModifiedBy;
        snack.DeletedBy = source.DeletedBy;
        snack.IsDeleted = source.IsDeleted;
        return snack;
    }
}
