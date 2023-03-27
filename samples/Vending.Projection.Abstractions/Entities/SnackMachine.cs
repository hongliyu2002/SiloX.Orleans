using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Entities;

/// <summary>
///     Represents a snack machine that sells snacks.
/// </summary>
[PublicAPI]
[Serializable]
public sealed class SnackMachine
{
    public SnackMachine()
    {
    }

    public SnackMachine(Guid id, int version, DateTimeOffset? createdAt, string? createdBy, DateTimeOffset? lastModifiedAt, string? lastModifiedBy,
                        DateTimeOffset? deletedAt, string? deletedBy, bool isDeleted, decimal amountInTransaction, int slotsCount, int snackCount,
                        int snackQuantity, decimal snackAmount, int boughtCount, decimal boughtAmount)
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
        AmountInTransaction = amountInTransaction;
        SlotsCount = slotsCount;
        SnackCount = snackCount;
        SnackQuantity = snackQuantity;
        SnackAmount = snackAmount;
        BoughtCount = boughtCount;
        BoughtAmount = boughtAmount;
    }

    /// <summary>
    ///     Gets or sets the ID of the snack machine.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the version of the snack machine.(Concurrency token)
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the snack machine was created.
    /// </summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the name of the user who created the snack machine.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the snack machine was last modified.
    /// </summary>
    public DateTimeOffset? LastModifiedAt { get; set; }

    /// <summary>
    ///     Gets or sets the name of the user who last modified the snack machine.
    /// </summary>
    public string? LastModifiedBy { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the snack machine was deleted.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    ///     Gets or sets the name of the user who deleted the snack machine.
    /// </summary>
    public string? DeletedBy { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this snack machine has been deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     Gets or sets the amount of money currently inside the snack machine.
    /// </summary>
    public Money MoneyInside { get; set; } = new();

    /// <summary>
    ///     Gets or sets the amount of money currently in the transaction.
    /// </summary>
    public decimal AmountInTransaction { get; set; }

    /// <summary>
    ///     Gets or sets the list of slots in the snack machine.
    /// </summary>
    public IList<Slot> Slots { get; set; } = new List<Slot>();

    /// <summary>
    ///     Gets or sets the number of slots in the snack machine.
    /// </summary>
    public int SlotsCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of distinct snacks in the snack machine.
    /// </summary>
    public int SnackCount { get; set; }

    /// <summary>
    ///     Gets or sets the total quantity of snacks in the snack machine.
    /// </summary>
    public int SnackQuantity { get; set; }

    /// <summary>
    ///     Gets or sets the total amount of money from snacks in the snack machine.
    /// </summary>
    public decimal SnackAmount { get; set; }

    /// <summary>
    ///     Gets or sets the number of times snacks have been bought from the snack machine.
    /// </summary>
    public int BoughtCount { get; set; }

    /// <summary>
    ///     Gets or sets the total amount of money spent on snacks from the snack machine.
    /// </summary>
    public decimal BoughtAmount { get; set; }
}
