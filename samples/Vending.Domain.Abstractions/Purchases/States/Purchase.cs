namespace Vending.Domain.Abstractions.Purchases;

/// <summary>
///     Represents a purchase of snack on a snack machine.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class Purchase
{
    /// <summary>
    ///     The unique identifier of the purchase.
    /// </summary>
    [Id(0)]
    public Guid Id { get; set; }

    /// <summary>
    ///     The snack machine id.
    /// </summary>
    [Id(1)]
    public Guid MachineId { get; set; }

    /// <summary>
    ///     The position of slot.
    /// </summary>
    [Id(2)]
    public int Position { get; set; }

    /// <summary>
    ///     The snack id.
    /// </summary>
    [Id(3)]
    public Guid SnackId { get; set; }

    /// <summary>
    ///     The purchase price.
    /// </summary>
    [Id(4)]
    public decimal BoughtPrice { get; set; }

    /// <summary>
    ///     The date and time when the snack was purchase.
    /// </summary>
    [Id(5)]
    public DateTimeOffset? BoughtAt { get; set; }

    /// <summary>
    ///     The user who purchase the snack.
    /// </summary>
    [Id(6)]
    public string? BoughtBy { get; set; }

    /// <summary>
    ///     Gets a value indicating whether the purchase is set;ialized.
    /// </summary>
    public bool IsInitialized => BoughtAt != null;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Purchase with Id:'{Id}' MachineId:'{MachineId}' Position:'{Position}' SnackId:'{SnackId}' BoughtPrice:'{BoughtPrice}' BoughtAt:'{BoughtAt}' BoughtBy:'{BoughtBy}'";
    }
}
