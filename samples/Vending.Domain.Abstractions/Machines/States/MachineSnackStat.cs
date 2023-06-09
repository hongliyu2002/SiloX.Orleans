﻿using Fluxera.Guards;

namespace Vending.Domain.Abstractions.Machines;

/// <summary>
///     The snack stats state.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class MachineSnackStat
{
    /// <summary>
    ///     Creates a new instance of the <see cref="MachineSnackStat" /> class.
    /// </summary>
    public MachineSnackStat()
    {
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="MachineSnackStat" /> class.
    /// </summary>
    /// <param name="machineId">The machine id.</param>
    /// <param name="snackId">The snack id.</param>
    /// <param name="totalQuantity">The total quantity of snacks in the machine.</param>
    /// <param name="totalAmount">The total amount of the snacks in the machine.</param>
    public MachineSnackStat(Guid machineId, Guid snackId, int totalQuantity, decimal totalAmount)
        : this()
    {
        MachineId = machineId;
        SnackId = snackId;
        TotalQuantity = Guard.Against.Negative(totalQuantity, nameof(totalQuantity));
        TotalAmount = Guard.Against.Negative(totalAmount, nameof(totalAmount));
    }

    /// <summary>
    ///     The machine id.
    /// </summary>
    [Id(0)]
    public Guid MachineId { get; set; }

    /// <summary>
    ///     The snack id.
    /// </summary>
    [Id(1)]
    public Guid SnackId { get; set; }

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
        return $"MachineSnackStat with MachineId:'{MachineId}' SnackId:'{SnackId}' TotalQuantity:'{TotalQuantity}' BoughtAmount:'{TotalAmount}'";
    }
}
