namespace Vending.Domain.Abstractions.Machines;

/// <summary>
///     The machines stats.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class StatsOfMachines
{
    /// <summary>
    ///     Count of snacks machines.
    /// </summary>
    [Id(0)]
    public int MachineCount { get; set; }

    /// <summary>
    ///     The quantity of snacks in snack mMachines.
    /// </summary>
    [Id(1)]
    public int TotalQuantity { get; set; }

    /// <summary>
    ///     The amount of snacks in snack mMachines.
    /// </summary>
    [Id(2)]
    public decimal TotalAmount { get; set; }
}
