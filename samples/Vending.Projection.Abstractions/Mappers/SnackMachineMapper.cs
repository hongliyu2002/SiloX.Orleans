using Vending.Projection.Abstractions.Entities;

namespace Vending.Projection.Abstractions.Mappers;

public static class SnackMachineMapper
{
    /// <summary>
    ///     Converts a <see cref="Domain.Abstractions.States.SnackMachine" /> to a <see cref="SnackMachine" />.
    /// </summary>
    /// <param name="machineInGrain">The <see cref="Domain.Abstractions.States.SnackMachine" /> to convert.</param>
    /// <param name="getSnackNamePicture">A function that returns the name and picture of a snack.</param>
    /// <param name="machine">The <see cref="SnackMachine" /> to convert to.</param>
    /// <returns>The <see cref="SnackMachine" /> converted from the <see cref="Domain.Abstractions.States.SnackMachine" />.</returns>
    public static async Task<SnackMachine> ToProjection(this Domain.Abstractions.States.SnackMachine machineInGrain, Func<Guid, Task<(string, string?)>> getSnackNamePicture, SnackMachine? machine = null)
    {
        machine ??= new SnackMachine();
        machine.Id = machineInGrain.Id;
        machine.CreatedAt = machineInGrain.CreatedAt;
        machine.LastModifiedAt = machineInGrain.LastModifiedAt;
        machine.DeletedAt = machineInGrain.DeletedAt;
        machine.CreatedBy = machineInGrain.CreatedBy;
        machine.LastModifiedBy = machineInGrain.LastModifiedBy;
        machine.DeletedBy = machineInGrain.DeletedBy;
        machine.IsDeleted = machineInGrain.IsDeleted;
        machine.MoneyInside = machineInGrain.MoneyInside.ToProjection(machine.MoneyInside);
        machine.AmountInTransaction = machineInGrain.AmountInTransaction;
        machine.Slots = await Task.WhenAll(machineInGrain.Slots.Select(slot => slot.ToProjection(getSnackNamePicture, machine.Slots.FirstOrDefault(sl => sl.MachineId == slot.MachineId && sl.Position == slot.Position))));
        machine.SlotsCount = machineInGrain.SlotsCount;
        machine.SnackCount = machineInGrain.SnackCount;
        machine.SnackQuantity = machineInGrain.SnackQuantity;
        machine.SnackAmount = machineInGrain.SnackAmount;
        return machine;
    }
}
