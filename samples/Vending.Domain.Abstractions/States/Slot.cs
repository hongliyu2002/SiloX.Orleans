namespace Vending.Domain.Abstractions.States;

/// <summary>
///     The slot state.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class Slot
{
    /// <summary>
    ///     The machine id.
    /// </summary>
    [Id(0)]
    public Guid MachineId { get; set; }

    /// <summary>
    ///     The position of the slot.
    /// </summary>
    [Id(1)]
    public int Position { get; set; }

    /// <summary>
    ///     The snack pile.
    /// </summary>
    [Id(2)]
    public SnackPile? SnackPile { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Slot with Position:{Position} SnackPile:'{SnackPile}'";
    }
}
