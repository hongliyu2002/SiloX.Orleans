using Vending.Domain.Abstractions.Machines;

namespace Vending.Projection.Abstractions.Machines;

public static class MoneyMapper
{
    /// <summary>
    ///     Maps a <see cref="Domain.Abstractions.Machines.Money" /> to a <see cref="MoneyInfo" />.
    /// </summary>
    /// <param name="money">The MoneyInGrain to convert. </param>
    /// <param name="moneyInfo">The MoneyInfo projection to convert to. </param>
    /// <returns>The MoneyInfo projection. </returns>
    public static MoneyInfo ToProjection(this Money money, MoneyInfo? moneyInfo = null)
    {
        moneyInfo ??= new MoneyInfo();
        moneyInfo.Yuan1 = money.Yuan1;
        moneyInfo.Yuan2 = money.Yuan2;
        moneyInfo.Yuan5 = money.Yuan5;
        moneyInfo.Yuan10 = money.Yuan10;
        moneyInfo.Yuan20 = money.Yuan20;
        moneyInfo.Yuan50 = money.Yuan50;
        moneyInfo.Yuan100 = money.Yuan100;
        moneyInfo.Amount = money.Amount;
        return moneyInfo;
    }
}
