using Fluxera.Guards;
using Orleans.FluentResults;

namespace Vending.Domain.Abstractions.SnackMachines;

/// <summary>
///     Represents money in the snack machine.
/// </summary>
/// <param name="Yuan1">The number of ￥1 coins. </param>
/// <param name="Yuan2">The number of ￥2 coins. </param>
/// <param name="Yuan5">The number of ￥5 coins. </param>
/// <param name="Yuan10">The number of ￥10 notes. </param>
/// <param name="Yuan20">The number of ￥20 notes. </param>
/// <param name="Yuan50">The number of ￥50 notes. </param>
/// <param name="Yuan100">The number of ￥100 notes. </param>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record Money(int Yuan1, int Yuan2, int Yuan5, int Yuan10, int Yuan20, int Yuan50, int Yuan100)
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
    ///     The total amount of money.
    /// </summary>
    public decimal Amount => Yuan1 * 1m + Yuan2 * 2m + Yuan5 * 5m + Yuan10 * 10m + Yuan20 * 20m + Yuan50 * 50m + Yuan100 * 100m;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Money ￥1:{Yuan1} ￥2:{Yuan2} ￥5:{Yuan5} ￥10:{Yuan10} ￥20:{Yuan20} ￥50:{Yuan50} ￥100:{Yuan100} Amount:{Amount}";
    }

    #region Create

    /// <summary>
    ///     Creates a new instance of <see cref="Money" />.
    /// </summary>
    public static Result<Money> Create(int yuan1, int yuan2, int yuan5, int yuan10, int yuan20, int yuan50, int yuan100)
    {
        return Result.Ok()
                     .Verify(yuan1 >= 0, "￥1 should not be negative.")
                     .Verify(yuan2 >= 0, "￥2 should not be negative.")
                     .Verify(yuan5 >= 0, "￥5 should not be negative.")
                     .Verify(yuan10 >= 0, "￥10 should not be negative.")
                     .Verify(yuan20 >= 0, "￥20 should not be negative.")
                     .Verify(yuan50 >= 0, "￥50 should not be negative.")
                     .Verify(yuan100 >= 0, "￥100 should not be negative.")
                     .MapTry(() => new Money(yuan1, yuan2, yuan5, yuan10, yuan20, yuan50, yuan100));
    }

    #endregion

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

    #region Operator

    public static Money operator +(Money money, Money money2)
    {
        money = Guard.Against.Null(money, nameof(money));
        money2 = Guard.Against.Null(money2, nameof(money2));
        var result = Create(money.Yuan1 + money2.Yuan1, money.Yuan2 + money2.Yuan2, money.Yuan5 + money2.Yuan5, money.Yuan10 + money2.Yuan10, money.Yuan20 + money2.Yuan20, money.Yuan50 + money2.Yuan50, money.Yuan100 + money2.Yuan100);
        return result.IsSuccess ? result.Value : throw new InvalidOperationException(result.ToString());
    }

    public static Money operator -(Money money, Money money2)
    {
        money = Guard.Against.Null(money, nameof(money));
        money2 = Guard.Against.Null(money2, nameof(money2));
        var result = Create(money.Yuan1 - money2.Yuan1, money.Yuan2 - money2.Yuan2, money.Yuan5 - money2.Yuan5, money.Yuan10 - money2.Yuan10, money.Yuan20 - money2.Yuan20, money.Yuan50 - money2.Yuan50, money.Yuan100 - money2.Yuan100);
        return result.IsSuccess ? result.Value : throw new InvalidOperationException(result.ToString());
    }

    public static Money operator *(Money money, int multiplier)
    {
        money = Guard.Against.Null(money, nameof(money));
        multiplier = Guard.Against.Negative(multiplier, nameof(multiplier));
        var result = Create(money.Yuan1 * multiplier, money.Yuan2 * multiplier, money.Yuan5 * multiplier, money.Yuan10 * multiplier, money.Yuan20 * multiplier, money.Yuan50 * multiplier, money.Yuan100 * multiplier);
        return result.IsSuccess ? result.Value : throw new InvalidOperationException(result.ToString());
    }

    #endregion

}
