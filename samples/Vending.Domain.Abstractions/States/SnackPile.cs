using Orleans.FluentResults;

namespace Vending.Domain.Abstractions.States;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackPile(Guid SnackId, int Quantity, decimal Price)
{
    public decimal TotalPrice => Quantity * Price;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"SnackPile with SnackId:'{SnackId}' Quantity:{Quantity} Price:{Price} TotalPrice:{TotalPrice}";
    }

    #region Create

    public static Result<SnackPile> Create(Guid snackId, int quantity, decimal price)
    {
        return Result.Ok()
                     .Verify(snackId != Guid.Empty, "Snack id cannot be empty.")
                     .Verify(quantity >= 0, "Quantity cannot be negative.")
                     .Verify(price >= 0, "Price cannot be negative.")
                     .Verify(price % 0.01m == 0, "The decimal portion of the price cannot be less than 0.01.")
                     .MapTry(() => new SnackPile(snackId, quantity, price));
    }

    #endregion

    #region Pop

    public bool CanPopOne(out SnackPile? snackPile)
    {
        if (Quantity < 1)
        {
            snackPile = null;
            return false;
        }
        snackPile = this;
        return true;
    }

    #endregion

}
