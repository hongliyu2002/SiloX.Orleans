using Fluxera.Guards;

namespace Vending.Domain.Abstractions.SnackMachines;

/// <summary>
///     Represents money in the snack machine.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class Money
{
    public static readonly Money Zero = new(0, 0, 0, 0, 0, 0, 0);
    public static readonly Money OneYuan = new(1, 0, 0, 0, 0, 0, 0);
    public static readonly Money TwoYuan = new(0, 1, 0, 0, 0, 0, 0);
    public static readonly Money FiveYuan = new(0, 0, 1, 0, 0, 0, 0);
    public static readonly Money TenYuan = new(0, 0, 0, 1, 0, 0, 0);
    public static readonly Money TwentyYuan = new(0, 0, 0, 0, 1, 0, 0);
    public static readonly Money FiftyYuan = new(0, 0, 0, 0, 0, 1, 0);
    public static readonly Money OneHundredYuan = new(0, 0, 0, 0, 0, 0, 1);
    public static readonly Money[] CoinsAndNotes =
    {
        OneYuan,
        TwoYuan,
        FiveYuan,
        TenYuan,
        TwentyYuan,
        FiftyYuan,
        OneHundredYuan
    };

    /// <summary>
    ///     Represents money in the snack machine.
    /// </summary>
    public Money()
    {
    }

    /// <summary>
    ///     Represents money in the snack machine.
    /// </summary>
    /// <param name="yuan1">The number of ￥1 coins. </param>
    /// <param name="yuan2">The number of ￥2 coins. </param>
    /// <param name="yuan5">The number of ￥5 coins. </param>
    /// <param name="yuan10">The number of ￥10 notes. </param>
    /// <param name="yuan20">The number of ￥20 notes. </param>
    /// <param name="yuan50">The number of ￥50 notes. </param>
    /// <param name="yuan100">The number of ￥100 notes. </param>
    public Money(int yuan1, int yuan2, int yuan5, int yuan10, int yuan20, int yuan50, int yuan100)
        : this()
    {
        Yuan1 = Guard.Against.Negative(yuan1, nameof(yuan1));
        Yuan2 = Guard.Against.Negative(yuan2, nameof(yuan2));
        Yuan5 = Guard.Against.Negative(yuan5, nameof(yuan5));
        Yuan10 = Guard.Against.Negative(yuan10, nameof(yuan10));
        Yuan20 = Guard.Against.Negative(yuan20, nameof(yuan20));
        Yuan50 = Guard.Against.Negative(yuan50, nameof(yuan50));
        Yuan100 = Guard.Against.Negative(yuan100, nameof(yuan100));
        Amount = yuan1 * 1m + yuan2 * 2m + yuan5 * 5m + yuan10 * 10m + yuan20 * 20m + yuan50 * 50m + yuan100 * 100m;
    }

    /// <summary>
    ///     The number of ￥1 coins.
    /// </summary>
    [Id(0)]
    public int Yuan1 { get; private set; }

    /// <summary>
    ///     The number of ￥2 coins.
    /// </summary>
    [Id(1)]
    public int Yuan2 { get; private set; }

    /// <summary>
    ///     The number of ￥5 coins.
    /// </summary>
    [Id(2)]
    public int Yuan5 { get; private set; }

    /// <summary>
    ///     The number of ￥10 notes.
    /// </summary>
    [Id(3)]
    public int Yuan10 { get; private set; }

    /// <summary>
    ///     The number of ￥20 notes.
    /// </summary>
    [Id(4)]
    public int Yuan20 { get; private set; }

    /// <summary>
    ///     The number of ￥50 notes.
    /// </summary>
    [Id(5)]
    public int Yuan50 { get; private set; }

    /// <summary>
    ///     The number of ￥100 notes.
    /// </summary>
    [Id(6)]
    public int Yuan100 { get; private set; }

    /// <summary>
    ///     The total amount of money.
    /// </summary>
    [Id(7)]
    public decimal Amount { get; private set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Money ￥1:{Yuan1} ￥2:{Yuan2} ￥5:{Yuan5} ￥10:{Yuan10} ￥20:{Yuan20} ￥50:{Yuan50} ￥100:{Yuan100} Amount:{Amount}";
    }

    #region Allocate

    /// <summary>
    ///     Allocates the specified amount of money.
    /// </summary>
    /// <param name="amount">The amount of money to allocate. </param>
    /// <param name="moneyToReturn">The money to return. </param>
    /// <returns><see langword="true" /> if the money can be allocated; otherwise, <see langword="false" />.</returns>
    public bool CanAllocate(decimal amount, out Money moneyToReturn)
    {
        if (amount < 0)
        {
            moneyToReturn = Zero;
            return false;
        }
        moneyToReturn = AllocateCore(amount);
        return moneyToReturn.Amount == amount;
    }

    /// <summary>
    ///     Allocates the specified amount of money.
    /// </summary>
    private Money AllocateCore(decimal amount)
    {
        var yuan100 = Math.Min((int)(amount / 100m), Yuan100);
        amount -= yuan100 * 100m;
        var yuan50 = Math.Min((int)(amount / 50m), Yuan50);
        amount -= yuan50 * 50m;
        var yuan20 = Math.Min((int)(amount / 20m), Yuan20);
        amount -= yuan20 * 20m;
        var yuan10 = Math.Min((int)(amount / 10m), Yuan10);
        amount -= yuan10 * 10m;
        var yuan5 = Math.Min((int)(amount / 5m), Yuan5);
        amount -= yuan5 * 5m;
        var yuan2 = Math.Min((int)(amount / 2m), Yuan2);
        amount -= yuan2 * 2m;
        var yuan1 = Math.Min((int)(amount / 1m), Yuan1);
        // amount -= yuan1 * 1m;
        return new Money(yuan1, yuan2, yuan5, yuan10, yuan20, yuan50, yuan100);
    }

    #endregion

    #region Operators

    public void Add(Money money)
    {
        money = Guard.Against.Null(money, nameof(money));
        Yuan1 += money.Yuan1;
        Yuan2 += money.Yuan2;
        Yuan5 += money.Yuan5;
        Yuan10 += money.Yuan10;
        Yuan20 += money.Yuan20;
        Yuan50 += money.Yuan50;
        Yuan100 += money.Yuan100;
        Amount += money.Amount;
    }

    public void Subtract(Money money)
    {
        money = Guard.Against.Null(money, nameof(money));
        Guard.Against.Negative(Yuan1 - money.Yuan1, nameof(money.Yuan1));
        Guard.Against.Negative(Yuan2 - money.Yuan2, nameof(money.Yuan2));
        Guard.Against.Negative(Yuan5 - money.Yuan5, nameof(money.Yuan5));
        Guard.Against.Negative(Yuan10 - money.Yuan10, nameof(money.Yuan10));
        Guard.Against.Negative(Yuan20 - money.Yuan20, nameof(money.Yuan20));
        Guard.Against.Negative(Yuan50 - money.Yuan50, nameof(money.Yuan50));
        Guard.Against.Negative(Yuan100 - money.Yuan100, nameof(money.Yuan100));
        Guard.Against.Negative(Amount - money.Amount, nameof(money.Amount));
        Yuan1 -= money.Yuan1;
        Yuan2 -= money.Yuan2;
        Yuan5 -= money.Yuan5;
        Yuan10 -= money.Yuan10;
        Yuan20 -= money.Yuan20;
        Yuan50 -= money.Yuan50;
        Yuan100 -= money.Yuan100;
        Amount -= money.Amount;
    }

    #endregion

}
