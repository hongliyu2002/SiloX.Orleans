namespace Vending.Domain.Abstractions.States;

/// <summary>
///     Represents a purchase of snack on a snack machine.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class SnackMachineSnackPurchase
{
    /// <summary>
    ///     The snack machine id.
    /// </summary>
    [Id(0)]
    public Guid MachineId { get; set; }

    /// <summary>
    ///     The position of slot.
    /// </summary>
    [Id(1)]
    public int Position { get; set; }

    /// <summary>
    ///     The snack id.
    /// </summary>
    [Id(2)]
    public Guid SnackId { get; set; }

    /// <summary>
    ///     The purchase price.
    /// </summary>
    [Id(3)]
    public decimal BoughtPrice { get; set; }

    /// <summary>
    ///     The date and time when the snack was purchase.
    /// </summary>
    [Id(4)]
    public DateTimeOffset? BoughtAt { get; set; }

    /// <summary>
    ///     The user who purchase the snack.
    /// </summary>
    [Id(5)]
    public string? BoughtBy { get; set; }

    /// <summary>
    ///     Gets a value indicating whether the purchase is set;ialized.
    /// </summary>
    public bool IsInitialized => BoughtAt != null;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"SnackMachineSnackPurchase with MachineId:'{MachineId}' Position:'{Position}' SnackId:'{SnackId}' BoughtPrice:'{BoughtPrice}' BoughtAt:'{BoughtAt}' BoughtBy:'{BoughtBy}'";
    }
}
