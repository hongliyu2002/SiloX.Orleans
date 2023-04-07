using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Machines;

/// <summary>
///     Represents a snack machine slot that holds a <see cref="SnackPile" />.
/// </summary>
[PublicAPI]
[Serializable]
[GenerateSerializer]
public sealed class MachineSlotInfo
{
    /// <summary>
    ///     The ID of the snack machine that this slot belongs to.
    /// </summary>
    [Id(0)]
    public Guid MachineId { get; set; }

    /// <summary>
    ///     The position of this slot within the snack machine.
    /// </summary>
    [Id(1)]
    public int Position { get; set; }

    /// <summary>
    ///     The <see cref="SnackPile" /> that is stored in this slot.
    /// </summary>
    [Id(2)]
    public SnackPileInfo? SnackPile { get; set; }
}
