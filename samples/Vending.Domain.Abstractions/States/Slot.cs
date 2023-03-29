namespace Vending.Domain.Abstractions.States;

[Serializable]
[GenerateSerializer]
public sealed class Slot
{
    [Id(0)]
    public int Position { get; set; }

    [Id(1)]
    public SnackPile? SnackPile { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Slot with Position:{Position} SnackPile:'{SnackPile}'";
    }
}
