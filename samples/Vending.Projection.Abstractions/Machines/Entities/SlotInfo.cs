using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Machines;

/// <summary>
///     Represents a snack machine machineSlot that holds a <see cref="SnackPile" />.
/// </summary>

[Serializable]
[GenerateSerializer]
public sealed class SlotInfo
{
    /// <summary>
    ///     Gets or sets the ID of the snack machine that this machineSlot belongs to.
    /// </summary>
    [Id(0)]
    public Guid MachineId { get; set; }

    /// <summary>
    ///     Gets or sets the position of this machineSlot within the snack machine.
    /// </summary>
    [Id(1)]
    public int Position { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="SnackPile" /> that is stored in this machineSlot.
    /// </summary>
    [Id(2)]
    public SnackPileInfo? SnackPile { get; set; }
}
