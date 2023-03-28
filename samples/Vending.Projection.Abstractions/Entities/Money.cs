using Fluxera.Guards;
using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Entities;

/// <summary>
///     Represents an entity class for storing information about money.
/// </summary>
[PublicAPI]
[Serializable]
public sealed class Money
{
    public Money()
    {
    }

    public Money(int yuan1, int yuan2, int yuan5, int yuan10, int yuan20, int yuan50, int yuan100)
    {
        Yuan1 = yuan1;
        Yuan2 = yuan2;
        Yuan5 = yuan5;
        Yuan10 = yuan10;
        Yuan20 = yuan20;
        Yuan50 = yuan50;
        Yuan100 = yuan100;
        Amount = Yuan1 * 1m + Yuan2 * 2m + Yuan5 * 5m + Yuan10 * 10m + Yuan20 * 20m + Yuan50 * 50m + Yuan100 * 100m;
    }

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

    #region Operator

    public static Money operator +(Money money1, Money money2)
    {
        money1 = Guard.Against.Null(money1);
        money2 = Guard.Against.Null(money2);
        return new Money(money1.Yuan1 + money2.Yuan1, money1.Yuan2 + money2.Yuan2, money1.Yuan5 + money2.Yuan5, money1.Yuan10 + money2.Yuan10, money1.Yuan20 + money2.Yuan20, money1.Yuan50 + money2.Yuan50, money1.Yuan100 + money2.Yuan100);
    }

    public static Money operator -(Money money1, Money money2)
    {
        money1 = Guard.Against.Null(money1);
        money2 = Guard.Against.Null(money2);
        return new Money(money1.Yuan1 - money2.Yuan1, money1.Yuan2 - money2.Yuan2, money1.Yuan5 - money2.Yuan5, money1.Yuan10 - money2.Yuan10, money1.Yuan20 - money2.Yuan20, money1.Yuan50 - money2.Yuan50, money1.Yuan100 - money2.Yuan100);
    }

    public static Money operator *(Money money1, int multiplier)
    {
        money1 = Guard.Against.Null(money1);
        multiplier = Guard.Against.Negative(multiplier, nameof(multiplier));
        return new Money(money1.Yuan1 * multiplier, money1.Yuan2 * multiplier, money1.Yuan5 * multiplier, money1.Yuan10 * multiplier, money1.Yuan20 * multiplier, money1.Yuan50 * multiplier, money1.Yuan100 * multiplier);
    }

    #endregion

}
