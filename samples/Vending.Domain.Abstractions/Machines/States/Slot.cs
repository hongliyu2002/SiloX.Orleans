using Fluxera.Guards;

namespace Vending.Domain.Abstractions.Machines;

/// <summary>
///     The slot state.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class Slot
{
    /// <summary>
    ///     Creates a new instance of the <see cref="Slot" /> class.
    /// </summary>
    public Slot()
    {
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="Slot" /> class.
    /// </summary>
    /// <param name="machineId">The machine id.</param>
    /// <param name="position">The position of the slot.</param>
    /// <param name="snackPile">The snack pile.</param>
    public Slot(Guid machineId, int position, SnackPile? snackPile = null)
        : this()
    {
        MachineId = Guard.Against.Empty(machineId, nameof(machineId));
        Position = Guard.Against.Negative(position, nameof(position));
        SnackPile = snackPile;
    }

    /// <summary>
    ///     The machine id.
    /// </summary>
    [Id(0)]
    public Guid MachineId { get; private set; }

    /// <summary>
    ///     The position of the slot.
    /// </summary>
    [Id(1)]
    public int Position { get; private set; }

    /// <summary>
    ///     The snack pile.
    /// </summary>
    [Id(2)]
    public SnackPile? SnackPile { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Slot with MachineId:'{MachineId}' Position:'{Position}' SnackPile:'{SnackPile}'";
    }
}
