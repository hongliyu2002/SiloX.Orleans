using Vending.Projection.Abstractions.Entities;

namespace Vending.Projection.Abstractions.Mappers;

public static class MoneyMapper
{
    public static Money ToProjection(this Domain.Abstractions.States.Money source, Money? money = null)
    {
        money ??= new Money();
        money.Yuan1 = source.Yuan1;
        money.Yuan2 = source.Yuan2;
        money.Yuan5 = source.Yuan5;
        money.Yuan10 = source.Yuan10;
        money.Yuan20 = source.Yuan20;
        money.Yuan50 = source.Yuan50;
        money.Yuan100 = source.Yuan100;
        money.Amount = source.Amount;
        return money;
    }

    public static Domain.Abstractions.States.Money ToDomain(this Money source)
    {
        return new Domain.Abstractions.States.Money(source.Yuan1, source.Yuan2, source.Yuan5, source.Yuan10, source.Yuan20, source.Yuan50, source.Yuan100);
    }
}
