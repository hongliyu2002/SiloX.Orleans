namespace Vending.Domain.Abstractions.Machines;

/// <summary>
///     The snack machine stats.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class MachineStat
{
    /// <summary>
    ///     Count of snacks machines.
    /// </summary>
    [Id(0)]
    public int Count { get; set; }
}
