using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Entities;

/// <summary>
///     Represents a record of a snack that has been bought from a vending machine.
/// </summary>
[PublicAPI]
[Serializable]
public sealed class SnackBought
{
    public SnackBought()
    {
    }

    public SnackBought(Guid machineId, int position, Guid snackId, string snackName, string? snackPictureUrl, decimal boughtPrice, DateTimeOffset? boughtAt,
                       string? boughtBy)
    {
        MachineId = machineId;
        Position = position;
        SnackId = snackId;
        SnackName = snackName;
        SnackPictureUrl = snackPictureUrl;
        BoughtPrice = boughtPrice;
        BoughtAt = boughtAt;
        BoughtBy = boughtBy;
    }

    /// <summary>
    ///     Gets or sets the ID of the vending machine where the snack was bought.
    /// </summary>
    public Guid MachineId { get; set; }

    /// <summary>
    ///     Gets or sets the position of the slot in the vending machine where the snack was bought.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    ///     Gets or sets the ID of the snack that was bought.
    /// </summary>
    public Guid SnackId { get; set; }

    /// <summary>
    ///     Gets or sets the name of the snack that was bought.
    /// </summary>
    public string SnackName { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the URL of the picture for the snack that was bought.
    /// </summary>
    public string? SnackPictureUrl { get; set; }

    /// <summary>
    ///     Gets or sets the price of the snack that was bought.
    /// </summary>
    public decimal BoughtPrice { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the snack was bought.
    /// </summary>
    public DateTimeOffset? BoughtAt { get; set; }

    /// <summary>
    ///     Gets or sets the name of the user who bought the snack.
    /// </summary>
    public string? BoughtBy { get; set; }
}
