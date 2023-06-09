﻿using Fluxera.Guards;

namespace Vending.Domain.Abstractions.Machines;

/// <summary>
///     The snack pile state.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class SnackPile
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SnackPile" /> class.
    /// </summary>
    public SnackPile()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SnackPile" /> class.
    /// </summary>
    /// <param name="snackId">The snack id.</param>
    /// <param name="quantity">The quantity of snacks in the pile.</param>
    /// <param name="price">The price of the snack. This is the price of a single snack, not the total price of the pile.</param>
    public SnackPile(Guid snackId, int quantity, decimal price)
        : this()
    {
        SnackId = snackId;
        Quantity = Guard.Against.Negative(quantity, nameof(quantity));
        Price = Guard.Against.Negative(price, nameof(price));
        Amount = Quantity * Price;
    }

    /// <summary>
    ///     The snack id.
    /// </summary>
    [Id(0)]
    public Guid SnackId { get; set; }

    /// <summary>
    ///     The quantity of snacks in the pile.
    /// </summary>
    [Id(1)]
    public int Quantity { get; set; }

    /// <summary>
    ///     The price of the snack. This is the price of a single snack, not the total price of the pile.
    /// </summary>
    [Id(2)]
    public decimal Price { get; set; }

    /// <summary>
    ///     The amount of the snack pile.
    /// </summary>
    [Id(3)]
    public decimal Amount { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"SnackPile with SnackId:'{SnackId}' Quantity:{Quantity} Price:{Price} BoughtAmount:{Amount}";
    }

    /// <summary>
    ///     Determines whether the pile can pop one.
    /// </summary>
    /// <returns><c>true</c> if the pile can pop one; otherwise, <c>false</c>.</returns>
    public bool CanSubtractOne()
    {
        return Quantity >= 1;
    }

    #region Operations

    public void Add(int quantity)
    {
        quantity = Guard.Against.Negative(quantity, nameof(quantity));
        Quantity += quantity;
        Amount = Quantity * Price;
    }

    public void Subtract(int quantity)
    {
        quantity = Guard.Against.Negative(quantity, nameof(quantity));
        Guard.Against.Negative(Quantity - quantity, nameof(quantity));
        Quantity -= quantity;
        Amount = Quantity * Price;
    }

    #endregion

}
