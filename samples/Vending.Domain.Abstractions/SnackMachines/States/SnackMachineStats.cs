namespace Vending.Domain.Abstractions.SnackMachines;

/// <summary>
///     The snack machine stats.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class SnackMachineStats
{
    /// <summary>
    ///     Count of snacks machines.
    /// </summary>
    [Id(0)]
    public int Count { get; set; }
}
