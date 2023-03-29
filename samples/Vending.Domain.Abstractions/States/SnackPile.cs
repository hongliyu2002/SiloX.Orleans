using Fluxera.Guards;

namespace Vending.Domain.Abstractions.States;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackPile(Guid SnackId, int Quantity, decimal Price)
{
    public decimal TotalAmount => Quantity * Price;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"SnackPile with SnackId:'{SnackId}' Quantity:{Quantity} Price:{Price} TotalAmount:{TotalAmount}";
    }

    public bool CanPopOne()
    {
        return Quantity >= 1;
    }

    #region Operator

    public static SnackPile operator +(SnackPile snackPile, int quantity)
    {
        snackPile = Guard.Against.Null(snackPile, nameof(snackPile));
        quantity = Guard.Against.Negative(quantity, nameof(quantity));
        return snackPile with { Quantity = snackPile.Quantity + quantity };
    }

    public static SnackPile operator -(SnackPile snackPile, int quantity)
    {
        snackPile = Guard.Against.Null(snackPile);
        quantity = Guard.Against.Negative(quantity, nameof(quantity));
        return snackPile with { Quantity = snackPile.Quantity - quantity };
    }

    #endregion

}
