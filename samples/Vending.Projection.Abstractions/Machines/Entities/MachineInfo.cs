using Fluxera.Extensions.Hosting.Modules.Domain.Shared.Model;

namespace Vending.Projection.Abstractions.Machines;

/// <summary>
///     Represents a snack machine that sells snacks.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class MachineInfo : IAuditedObject, ISoftDeleteObject
{
    /// <summary>
    ///     Gets or sets the ID of the snack machine.
    /// </summary>
    [Id(0)]
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the version of the snack machine.(Concurrency token)
    /// </summary>
    [Id(1)]
    public int Version { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the snack machine was created.
    /// </summary>
    [Id(2)]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the name of the user who created the snack machine.
    /// </summary>
    [Id(3)]
    public string? CreatedBy { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the snack machine was last modified.
    /// </summary>
    [Id(4)]
    public DateTimeOffset? LastModifiedAt { get; set; }

    /// <summary>
    ///     Gets or sets the name of the user who last modified the snack machine.
    /// </summary>
    [Id(5)]
    public string? LastModifiedBy { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the snack machine was deleted.
    /// </summary>
    [Id(6)]
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    ///     Gets or sets the name of the user who deleted the snack machine.
    /// </summary>
    [Id(7)]
    public string? DeletedBy { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this snack machine has been deleted.
    /// </summary>
    [Id(8)]
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     Gets or sets the amount of money currently inside the snack machine.
    /// </summary>
    [Id(9)]
    public MoneyInfo MoneyInfoInside { get; set; } = new();

    /// <summary>
    ///     Gets or sets the amount of money currently in the transaction.
    /// </summary>
    [Id(10)]
    public decimal AmountInTransaction { get; set; }

    /// <summary>
    ///     Gets or sets the list of slots in the snack machine.
    /// </summary>
    [Id(11)]
    public IList<SlotInfo> Slots { get; set; } = new List<SlotInfo>();

    /// <summary>
    ///     Gets or sets the number of slots in the snack machine.
    /// </summary>
    [Id(12)]
    public int SlotsCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of distinct snacks in the snack machine.
    /// </summary>
    [Id(13)]
    public int SnackCount { get; set; }

    /// <summary>
    ///     Gets or sets the total quantity of snacks in the snack machine.
    /// </summary>
    [Id(14)]
    public int SnackQuantity { get; set; }

    /// <summary>
    ///     Gets or sets the total amount of money from snacks in the snack machine.
    /// </summary>
    [Id(15)]
    public decimal SnackAmount { get; set; }

    /// <summary>
    ///     Gets or sets the number of times snacks have been bought from the snack machine.
    /// </summary>
    [Id(16)]
    public int BoughtCount { get; set; }

    /// <summary>
    ///     Gets or sets the total amount of money spent on snacks from the snack machine.
    /// </summary>
    [Id(17)]
    public decimal BoughtAmount { get; set; }
}
