using Vending.Domain.Abstractions.Machines;

namespace Vending.Projection.Abstractions.Machines;

public static class MachineMapper
{
    /// <summary>
    ///     Converts a <see cref="Machine" /> to a <see cref="MachineInfo" />.
    /// </summary>
    /// <param name="machine">The <see cref="Machine" /> to convert.</param>
    /// <param name="getSnackNamePicture">A function that returns the name and picture of a snack.</param>
    /// <param name="machineInfo">The <see cref="MachineInfo" /> to convert to.</param>
    /// <returns>The <see cref="MachineInfo" /> converted from the <see cref="Machine" />.</returns>
    public static async Task<MachineInfo> ToProjection(this Machine machine, Func<Guid, Task<(string, string?)>> getSnackNamePicture, MachineInfo? machineInfo = null)
    {
        machineInfo ??= new MachineInfo();
        machineInfo.Id = machine.Id;
        machineInfo.MoneyInside = machine.MoneyInside.ToProjection(machineInfo.MoneyInside);
        machineInfo.AmountInTransaction = machine.AmountInTransaction;
        machineInfo.Slots = (await Task.WhenAll(machine.Slots.Select(slot => slot.ToProjection(getSnackNamePicture, machineInfo.Slots.FirstOrDefault(ms => ms.MachineId == slot.MachineId && ms.Position == slot.Position))))).ToList();
        machineInfo.CreatedAt = machine.CreatedAt;
        machineInfo.LastModifiedAt = machine.LastModifiedAt;
        machineInfo.DeletedAt = machine.DeletedAt;
        machineInfo.CreatedBy = machine.CreatedBy;
        machineInfo.LastModifiedBy = machine.LastModifiedBy;
        machineInfo.DeletedBy = machine.DeletedBy;
        machineInfo.IsDeleted = machine.IsDeleted;
        machineInfo.SlotCount = machine.SlotCount;
        machineInfo.SnackCount = machine.SnackCount;
        machineInfo.SnackQuantity = machine.SnackQuantity;
        machineInfo.SnackAmount = machine.SnackAmount;
        return machineInfo;
    }

    /// <summary>
    ///     Converts a <see cref="MachineInfo" /> to a <see cref="Machine" />.
    /// </summary>
    /// <param name="machineInfo">The <see cref="MachineInfo" /> to convert.</param>
    /// <param name="machine">The <see cref="Machine" /> to convert to.</param>
    /// <returns>The <see cref="Machine" /> converted from the <see cref="MachineInfo" />.</returns>
    public static Machine ToDomain(this MachineInfo machineInfo, Machine? machine = null)
    {
        machine ??= new Machine();
        machine.Id = machineInfo.Id;
        machine.MoneyInside = machineInfo.MoneyInside.ToDomain(machine.MoneyInside);
        machine.AmountInTransaction = machineInfo.AmountInTransaction;
        machine.Slots = machineInfo.Slots.Select(slot => slot.ToDomain(machine.Slots.FirstOrDefault(ms => ms.MachineId == slot.MachineId && ms.Position == slot.Position))).ToList();
        machine.CreatedAt = machineInfo.CreatedAt;
        machine.LastModifiedAt = machineInfo.LastModifiedAt;
        machine.DeletedAt = machineInfo.DeletedAt;
        machine.CreatedBy = machineInfo.CreatedBy;
        machine.LastModifiedBy = machineInfo.LastModifiedBy;
        machine.DeletedBy = machineInfo.DeletedBy;
        machine.IsDeleted = machineInfo.IsDeleted;
        machine.SlotCount = machineInfo.SlotCount;
        machine.SnackCount = machineInfo.SnackCount;
        machine.SnackQuantity = machineInfo.SnackQuantity;
        machine.SnackAmount = machineInfo.SnackAmount;
        return machine;
    }
}