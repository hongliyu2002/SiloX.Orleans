using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.SnackMachines;

/// <summary>
///     Represents a snack machine slot that holds a <see cref="SnackPile" />.
/// </summary>

[Serializable]
[GenerateSerializer]
public sealed class Slot
{
    /// <summary>
    ///     Gets or sets the ID of the snack machine that this slot belongs to.
    /// </summary>
    [Id(0)]
    public Guid MachineId { get; set; }

    /// <summary>
    ///     Gets or sets the position of this slot within the snack machine.
    /// </summary>
    [Id(1)]
    public int Position { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="SnackPile" /> that is stored in this slot.
    /// </summary>
    [Id(2)]
    public SnackPile? SnackPile { get; set; }
}
