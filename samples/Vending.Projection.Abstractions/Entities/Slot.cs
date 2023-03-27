using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Entities;

/// <summary>
///     Represents a vending machine slot that holds a <see cref="SnackPile" />.
/// </summary>
[PublicAPI]
[Serializable]
public sealed class Slot
{
    public Slot()
    {
    }

    public Slot(Guid machineId, int position, SnackPile? snackPile)
    {
        MachineId = machineId;
        Position = position;
        SnackPile = snackPile;
    }

    /// <summary>
    ///     Gets or sets the ID of the vending machine that this slot belongs to.
    /// </summary>
    public Guid MachineId { get; set; }

    /// <summary>
    ///     Gets or sets the position of this slot within the vending machine.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="SnackPile" /> that is stored in this slot.
    /// </summary>
    public SnackPile? SnackPile { get; set; }
}
