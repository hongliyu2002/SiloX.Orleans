namespace Vending.Domain.Abstractions.Purchases;

/// <summary>
///     The purchase stats.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class StatsOfPurchases
{
    /// <summary>
    ///     Count of purchases.
    /// </summary>
    [Id(0)]
    public int BoughtCount { get; set; }

    /// <summary>
    ///     Total amount of money spent.
    /// </summary>
    [Id(1)]
    public decimal BoughtAmount { get; set; }
}
