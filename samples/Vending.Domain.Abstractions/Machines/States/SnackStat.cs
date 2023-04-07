using Fluxera.Guards;

namespace Vending.Domain.Abstractions.Machines;

/// <summary>
///     The snack stats state.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class SnackStat
{
    /// <summary>
    ///     Creates a new instance of the <see cref="SnackStat" /> class.
    /// </summary>
    public SnackStat()
    {
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="SnackStat" /> class.
    /// </summary>
    /// <param name="machineId">The machine id.</param>
    /// <param name="snackId">The snack id.</param>
    /// <param name="totalQuantity">The total quantity of snacks in the machine.</param>
    /// <param name="totalAmount">The total amount of the snacks in the machine.</param>
    public SnackStat(Guid machineId, Guid snackId, int totalQuantity, decimal totalAmount)
        : this()
    {
        MachineId = Guard.Against.Empty(machineId, nameof(machineId));
        SnackId = Guard.Against.Empty(snackId, nameof(snackId));
        TotalQuantity = Guard.Against.Negative(totalQuantity, nameof(totalQuantity));
        TotalAmount = Guard.Against.Negative(totalAmount, nameof(totalAmount));
    }

    /// <summary>
    ///     The machine id.
    /// </summary>
    [Id(0)]
    public Guid MachineId { get; private set; }

    /// <summary>
    ///     The snack id.
    /// </summary>
    [Id(1)]
    public Guid SnackId { get; private set; }

    /// <summary>
    ///     The total quantity of snacks in the machine.
    /// </summary>
    [Id(2)]
    public int TotalQuantity { get; set; }

    /// <summary>
    ///     The total amount of the snacks in the machine.
    /// </summary>
    [Id(3)]
    public decimal TotalAmount { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"SnackStat with MachineId:'{MachineId}' SnackId:'{SnackId}' TotalQuantity:'{TotalQuantity}' Amount:'{TotalAmount}'";
    }
}
