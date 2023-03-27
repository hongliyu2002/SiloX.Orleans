using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Entities;

/// <summary>
///     Represents an entity class for storing information about money.
/// </summary>
[PublicAPI]
[Serializable]
public sealed class Money
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Money" /> class.
    /// </summary>
    public Money()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Money" /> class with the specified parameters.
    /// </summary>
    /// <param name="yuan1">The number of 1 yuan coins.</param>
    /// <param name="yuan2">The number of 2 yuan coins.</param>
    /// <param name="yuan5">The number of 5 yuan coins.</param>
    /// <param name="yuan10">The number of 10 yuan coins.</param>
    /// <param name="yuan20">The number of 20 yuan bills.</param>
    /// <param name="yuan50">The number of 50 yuan bills.</param>
    /// <param name="yuan100">The number of 100 yuan bills.</param>
    /// <param name="amount">The total amount of money.</param>
    public Money(int yuan1, int yuan2, int yuan5, int yuan10, int yuan20, int yuan50, int yuan100, decimal amount)
    {
        Yuan1 = yuan1;
        Yuan2 = yuan2;
        Yuan5 = yuan5;
        Yuan10 = yuan10;
        Yuan20 = yuan20;
        Yuan50 = yuan50;
        Yuan100 = yuan100;
        Amount = amount;
    }

    /// <summary>
    ///     Gets or sets the number of 1 yuan coins.
    /// </summary>
    public int Yuan1 { get; init; }

    /// <summary>
    ///     Gets or sets the number of 2 yuan coins.
    /// </summary>
    public int Yuan2 { get; init; }

    /// <summary>
    ///     Gets or sets the number of 5 yuan coins.
    /// </summary>
    public int Yuan5 { get; init; }

    /// <summary>
    ///     Gets or sets the number of 10 yuan coins.
    /// </summary>
    public int Yuan10 { get; init; }

    /// <summary>
    ///     Gets or sets the number of 20 yuan bills.
    /// </summary>
    public int Yuan20 { get; init; }

    /// <summary>
    ///     Gets or sets the number of 50 yuan bills.
    /// </summary>
    public int Yuan50 { get; init; }

    /// <summary>
    ///     Gets or sets the number of 100 yuan bills.
    /// </summary>
    public int Yuan100 { get; init; }

    /// <summary>
    ///     Gets or sets the total amount of money.
    /// </summary>
    public decimal Amount { get; init; }
}
