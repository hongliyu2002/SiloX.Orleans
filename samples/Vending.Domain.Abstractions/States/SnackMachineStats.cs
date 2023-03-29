namespace Vending.Domain.Abstractions.States;

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
