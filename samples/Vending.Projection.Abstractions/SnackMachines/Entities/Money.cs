using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.SnackMachines;

/// <summary>
///     Represents an entity class for storing information about money.
/// </summary>
[PublicAPI]
[Serializable]
public sealed class Money
{
    /// <summary>
    ///     Gets or sets the number of 1 yuan coins.
    /// </summary>
    public int Yuan1 { get; set; }

    /// <summary>
    ///     Gets or sets the number of 2 yuan coins.
    /// </summary>
    public int Yuan2 { get; set; }

    /// <summary>
    ///     Gets or sets the number of 5 yuan coins.
    /// </summary>
    public int Yuan5 { get; set; }

    /// <summary>
    ///     Gets or sets the number of 10 yuan coins.
    /// </summary>
    public int Yuan10 { get; set; }

    /// <summary>
    ///     Gets or sets the number of 20 yuan bills.
    /// </summary>
    public int Yuan20 { get; set; }

    /// <summary>
    ///     Gets or sets the number of 50 yuan bills.
    /// </summary>
    public int Yuan50 { get; set; }

    /// <summary>
    ///     Gets or sets the number of 100 yuan bills.
    /// </summary>
    public int Yuan100 { get; set; }

    /// <summary>
    ///     Gets or sets the total amount of money.
    /// </summary>
    public decimal Amount { get; set; }
}
