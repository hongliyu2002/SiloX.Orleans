using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Machines;

/// <summary>
///     Represents an entity class for storing information about money.
/// </summary>
[PublicAPI]
[Serializable]
[GenerateSerializer]
public sealed class MoneyInfo
{
    /// <summary>
    ///     The number of 1 yuan coins.
    /// </summary>
    [Id(0)]
    public int Yuan1 { get; set; }

    /// <summary>
    ///     The number of 2 yuan coins.
    /// </summary>
    [Id(1)]
    public int Yuan2 { get; set; }

    /// <summary>
    ///     The number of 5 yuan coins.
    /// </summary>
    [Id(2)]
    public int Yuan5 { get; set; }

    /// <summary>
    ///     The number of 10 yuan bills.
    /// </summary>
    [Id(3)]
    public int Yuan10 { get; set; }

    /// <summary>
    ///     The number of 20 yuan bills.
    /// </summary>
    [Id(4)]
    public int Yuan20 { get; set; }

    /// <summary>
    ///     The number of 50 yuan bills.
    /// </summary>
    [Id(5)]
    public int Yuan50 { get; set; }

    /// <summary>
    ///     The number of 100 yuan bills.
    /// </summary>
    [Id(6)]
    public int Yuan100 { get; set; }

    /// <summary>
    ///     The total amount of money.
    /// </summary>
    [Id(7)]
    public decimal Amount { get; set; }
}
