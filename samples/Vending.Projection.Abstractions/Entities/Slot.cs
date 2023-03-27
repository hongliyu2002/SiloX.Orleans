using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Entities;

[PublicAPI]
[Serializable]
public sealed class Slot
{
    public Guid MachineId { get; set; }

    public int Position { get; set; }

    public SnackPile? SnackPile { get; set; }
}
