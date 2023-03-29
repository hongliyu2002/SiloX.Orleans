using Vending.Projection.Abstractions.Entities;

namespace Vending.Projection.Abstractions.Mappers;

public static class MoneyMapper
{
    public static Money ToProjection(this Domain.Abstractions.States.Money moneyInGrain, Money? money = null)
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
