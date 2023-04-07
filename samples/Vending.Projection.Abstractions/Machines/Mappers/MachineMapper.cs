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
        machineInfo.CreatedAt = machine.CreatedAt;
        machineInfo.LastModifiedAt = machine.LastModifiedAt;
        machineInfo.DeletedAt = machine.DeletedAt;
        machineInfo.CreatedBy = machine.CreatedBy;
        machineInfo.LastModifiedBy = machine.LastModifiedBy;
        machineInfo.DeletedBy = machine.DeletedBy;
        machineInfo.IsDeleted = machine.IsDeleted;
        machineInfo.MoneyInfoInside = machine.MoneyInside.ToProjection(machineInfo.MoneyInfoInside);
        machineInfo.AmountInTransaction = machine.AmountInTransaction;
        machineInfo.Slots = await Task.WhenAll(machine.Slots.Select(slot => slot.ToProjection(getSnackNamePicture,
                                                                                                 machineInfo.Slots.FirstOrDefault(sl => sl.MachineId == slot.MachineId && sl.Position == slot.Position))));
        machineInfo.SlotsCount = machine.SlotsCount;
        machineInfo.SnackCount = machine.SnackCount;
        machineInfo.SnackQuantity = machine.SnackQuantity;
        machineInfo.SnackAmount = machine.SnackAmount;
        return machineInfo;
    }
}
