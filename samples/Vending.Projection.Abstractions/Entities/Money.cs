using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Entities;

[PublicAPI]
[Serializable]
public sealed class Money
{
    public int Yuan1 { get; init; }

    public int Yuan2 { get; init; }

    public int Yuan5 { get; init; }

    public int Yuan10 { get; init; }

    public int Yuan20 { get; init; }

    public int Yuan50 { get; init; }

    public int Yuan100 { get; init; }

    public decimal Amount { get; init; }
}
