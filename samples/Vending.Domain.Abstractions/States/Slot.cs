using Fluxera.Guards;

namespace Vending.Domain.Abstractions.States;

[GenerateSerializer]
public sealed class Slot
{
    public Slot()
    {
    }

    public Slot(Guid machineId, int position, SnackPile? snackPile = null)
        : this()
    {
        MachineId = Guard.Against.Empty(machineId, nameof(machineId));
        Position = Guard.Against.Negative(position, nameof(position));
        SnackPile = snackPile;
    }

    [Id(0)]
    public Guid MachineId { get; set; }

    [Id(1)]
    public int Position { get; set; }

    [Id(2)]
    public SnackPile? SnackPile { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Slot with Position:{Position} SnackPile:'{SnackPile}'";
    }
}
