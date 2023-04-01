namespace Vending.Projection.Abstractions.SnackMachines;

public static class MoneyMapper
{
    /// <summary>
    ///     Maps a <see cref="Domain.Abstractions.SnackMachines.Money" /> to a <see cref="Money" />.
    /// </summary>
    /// <param name="moneyInGrain">The MoneyInGrain to convert. </param>
    /// <param name="money">The Money projection to convert to. </param>
    /// <returns>The Money projection. </returns>
    public static Money ToProjection(this Domain.Abstractions.SnackMachines.Money moneyInGrain, Money? money = null)
    {
        money ??= new Money();
        money.Yuan1 = moneyInGrain.Yuan1;
        money.Yuan2 = moneyInGrain.Yuan2;
        money.Yuan5 = moneyInGrain.Yuan5;
        money.Yuan10 = moneyInGrain.Yuan10;
        money.Yuan20 = moneyInGrain.Yuan20;
        money.Yuan50 = moneyInGrain.Yuan50;
        money.Yuan100 = moneyInGrain.Yuan100;
        money.Amount = moneyInGrain.Amount;
        return money;
    }
}
