using Vending.Domain.Abstractions.Commands;

namespace Vending.Domain.Abstractions.States;

[Serializable]
[GenerateSerializer]
public sealed class Snack
{
    [Id(0)]
    public Guid Id { get; set; }

    [Id(1)]
    public DateTimeOffset? CreatedAt { get; set; }

    [Id(2)]
    public string? CreatedBy { get; set; }

    public bool IsCreated => CreatedAt != null;

    [Id(3)]
    public DateTimeOffset? LastModifiedAt { get; set; }

    [Id(4)]
    public string? LastModifiedBy { get; set; }

    [Id(5)]
    public DateTimeOffset? DeletedAt { get; set; }

    [Id(6)]
    public string? DeletedBy { get; set; }

    [Id(7)]
    public bool IsDeleted { get; set; }

    [Id(8)]
    public string Name { get; set; } = null!;

    [Id(9)]
    public string? PictureUrl { get; set; }

    public override string ToString()
    {
        return $"Snack with Id:{Id} Name:'{Name}' PictureUrl:'{PictureUrl}'";
    }

    #region Apply

    public void Apply(SnackInitializeCommand command)
    {
        Id = command.Id;
        Name = command.Name;
        PictureUrl = command.PictureUrl;
        CreatedAt = command.OperatedAt;
        CreatedBy = command.OperatedBy;
    }

    public void Apply(SnackRemoveCommand command)
    {
        DeletedAt = command.OperatedAt;
        DeletedBy = command.OperatedBy;
        IsDeleted = true;
    }

    public void Apply(SnackChangeNameCommand command)
    {
        Name = command.Name;
        LastModifiedAt = command.OperatedAt;
        LastModifiedBy = command.OperatedBy;
    }

    public void Apply(SnackChangePictureUrlCommand command)
    {
        PictureUrl = command.PictureUrl;
        LastModifiedAt = command.OperatedAt;
        LastModifiedBy = command.OperatedBy;
    }

    #endregion

}
