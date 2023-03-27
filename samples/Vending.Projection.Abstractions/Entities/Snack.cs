using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Entities;

/// <summary>
///     Represents a snack that can be sold in a vending machine.
/// </summary>
[PublicAPI]
[Serializable]
public sealed class Snack
{
    public Snack()
    {
    }

    public Snack(Guid id, int version, DateTimeOffset? createdAt, string? createdBy, DateTimeOffset? lastModifiedAt, string? lastModifiedBy,
                 DateTimeOffset? deletedAt, string? deletedBy, bool isDeleted, string name, string? pictureUrl, int machineCount, int boughtCount,
                 decimal boughtAmount)
    {
        Id = id;
        Version = version;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        LastModifiedAt = lastModifiedAt;
        LastModifiedBy = lastModifiedBy;
        DeletedAt = deletedAt;
        DeletedBy = deletedBy;
        IsDeleted = isDeleted;
        Name = name;
        PictureUrl = pictureUrl;
        MachineCount = machineCount;
        BoughtCount = boughtCount;
        BoughtAmount = boughtAmount;
    }

    /// <summary>
    ///     Gets or sets the ID of the snack.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the version of the snack.(Concurrency token)
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the snack was created.
    /// </summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the name of the user who created the snack.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the snack was last modified.
    /// </summary>
    public DateTimeOffset? LastModifiedAt { get; set; }

    /// <summary>
    ///     Gets or sets the name of the user who last modified the snack.
    /// </summary>
    public string? LastModifiedBy { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the snack was deleted.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    ///     Gets or sets the name of the user who deleted the snack.
    /// </summary>
    public string? DeletedBy { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this snack has been deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     Gets or sets the name of the snack.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the URL of the picture for the snack.
    /// </summary>
    public string? PictureUrl { get; set; }

    /// <summary>
    ///     Gets or sets the number of vending machines that have this snack.
    /// </summary>
    public int MachineCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of times this snack has been bought.
    /// </summary>
    public int BoughtCount { get; set; }

    /// <summary>
    ///     Gets or sets the total amount of money that has been spent on this snack.
    /// </summary>
    public decimal BoughtAmount { get; set; }
}
