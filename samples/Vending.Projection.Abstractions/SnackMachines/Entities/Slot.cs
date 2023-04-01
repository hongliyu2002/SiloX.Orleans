using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.SnackMachines;

/// <summary>
///     Represents a snack machine slot that holds a <see cref="SnackPile" />.
/// </summary>
[PublicAPI]
[Serializable]
public sealed class Slot
{
    /// <summary>
    ///     Gets or sets the ID of the snack machine that this slot belongs to.
    /// </summary>
    public Guid MachineId { get; set; }

    /// <summary>
    ///     Gets or sets the position of this slot within the snack machine.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="SnackPile" /> that is stored in this slot.
    /// </summary>
    public SnackPile? SnackPile { get; set; }
}
