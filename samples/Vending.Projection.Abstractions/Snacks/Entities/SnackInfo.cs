using Fluxera.Extensions.Hosting.Modules.Domain.Shared.Model;
using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Snacks;

/// <summary>
///     Represents a snack that can be sold in a snack machine.
/// </summary>
[PublicAPI]
[Serializable]
[GenerateSerializer]
public sealed class SnackInfo : IAuditedObject, ISoftDeleteObject
{
    /// <summary>
    ///     Gets or sets the ID of the snack.
    /// </summary>
    [Id(0)]
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the version of the snack.(Concurrency token)
    /// </summary>
    [Id(1)]
    public int Version { get; set; }

    /// <summary>
    ///     Gets or sets the name of the snack.
    /// </summary>
    [Id(2)]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the URL of the picture for the snack.
    /// </summary>
    [Id(3)]
    public string? PictureUrl { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the snack was created.
    /// </summary>
    [Id(4)]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the name of the user who created the snack.
    /// </summary>
    [Id(5)]
    public string? CreatedBy { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the snack was last modified.
    /// </summary>
    [Id(6)]
    public DateTimeOffset? LastModifiedAt { get; set; }

    /// <summary>
    ///     Gets or sets the name of the user who last modified the snack.
    /// </summary>
    [Id(7)]
    public string? LastModifiedBy { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the snack was deleted.
    /// </summary>
    [Id(8)]
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    ///     Gets or sets the name of the user who deleted the snack.
    /// </summary>
    [Id(9)]
    public string? DeletedBy { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this snack has been deleted.
    /// </summary>
    [Id(10)]
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     Gets or sets the number of snack machines that have this snack.
    /// </summary>
    [Id(11)]
    public int MachineCount { get; set; }

    /// <summary>
    ///     The total quantity of this snack in the machine.
    /// </summary>
    [Id(12)]
    public int TotalQuantity { get; set; }

    /// <summary>
    ///     The total amount of this snack in the machine.
    /// </summary>
    [Id(13)]
    public decimal TotalAmount { get; set; }

    /// <summary>
    ///     Gets or sets the number of times this snack has been bought.
    /// </summary>
    [Id(14)]
    public int BoughtCount { get; set; }

    /// <summary>
    ///     Gets or sets the total amount of money that has been spent on this snack.
    /// </summary>
    [Id(15)]
    public decimal BoughtAmount { get; set; }
}
