using Fluxera.Extensions.Hosting.Modules.Domain.Shared.Model;
using Vending.Domain.Abstractions.Commands;

namespace Vending.Domain.Abstractions.States;

/// <summary>
///     Represents a snack.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class Snack : IAuditedObject, ISoftDeleteObject
{
    /// <summary>
    ///     The unique identifier of the snack.
    /// </summary>
    [Id(0)]
    public Guid Id { get; set; }

    /// <summary>
    ///     The date and time when the snack was created.
    /// </summary>
    [Id(1)]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    ///     The user who created the snack.
    /// </summary>
    [Id(2)]
    public string? CreatedBy { get; set; }

    /// <summary>
    ///     Indicates whether the snack has been created.
    /// </summary>
    public bool IsCreated => CreatedAt != null;

    /// <summary>
    ///     The date and time when the snack was last modified.
    /// </summary>
    [Id(3)]
    public DateTimeOffset? LastModifiedAt { get; set; }

    /// <summary>
    ///     The user who last modified the snack.
    /// </summary>
    [Id(4)]
    public string? LastModifiedBy { get; set; }

    /// <summary>
    ///     The date and time when the snack was deleted.
    /// </summary>
    [Id(5)]
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    ///     The user who deleted the snack.
    /// </summary>
    [Id(6)]
    public string? DeletedBy { get; set; }

    /// <summary>
    ///     Indicates whether the snack has been deleted.
    /// </summary>
    [Id(7)]
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     The name of the snack.
    /// </summary>
    [Id(8)]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     The URL of the picture of the snack.
    /// </summary>
    [Id(9)]
    public string? PictureUrl { get; set; }

    public override string ToString()
    {
        return $"Snack with Id:'{Id}' Name:'{Name}' PictureUrl:'{PictureUrl}'";
    }

    #region Apply

    public void Apply(SnackInitializeCommand command)
    {
        Id = command.SnackId;
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
