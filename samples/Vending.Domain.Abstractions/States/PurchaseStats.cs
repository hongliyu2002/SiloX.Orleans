namespace Vending.Domain.Abstractions.States;

/// <summary>
///     The purchase stats.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class PurchaseStats
{
    /// <summary>
    ///     Count of purchases.
    /// </summary>
    [Id(0)]
    public int Count { get; set; }

    /// <summary>
    ///     Total amount of money spent.
    /// </summary>
    [Id(0)]
    public decimal Amount { get; set; }
}
