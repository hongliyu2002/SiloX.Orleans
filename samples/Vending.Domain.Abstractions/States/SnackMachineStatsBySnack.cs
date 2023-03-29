namespace Vending.Domain.Abstractions.States;

/// <summary>
///     The snack machine stats by snack.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class SnackMachineStatsBySnack
{
    /// <summary>
    ///     The machine count.
    /// </summary>
    [Id(1)]
    public int Count { get; set; }
}
