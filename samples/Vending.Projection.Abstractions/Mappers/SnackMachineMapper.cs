using Vending.Projection.Abstractions.Entities;

namespace Vending.Projection.Abstractions.Mappers;

public static class SnackMachineMapper
{
    /// <summary>
    ///     Converts a <see cref="Domain.Abstractions.States.SnackMachine" /> to a <see cref="SnackMachine" />.
    /// </summary>
    /// <param name="snackMachineInGrain">The <see cref="Domain.Abstractions.States.SnackMachine" /> to convert.</param>
    /// <param name="getSnackNamePicture">A function that returns the name and picture of a snack.</param>
    /// <param name="snackMachine">The <see cref="SnackMachine" /> to convert to.</param>
    /// <returns>The <see cref="SnackMachine" /> converted from the <see cref="Domain.Abstractions.States.SnackMachine" />.</returns>
    public static async Task<SnackMachine> ToProjection(this Domain.Abstractions.States.SnackMachine snackMachineInGrain, Func<Guid, Task<(string, string?)>> getSnackNamePicture, SnackMachine? snackMachine = null)
    {
        snackMachine ??= new SnackMachine();
        snackMachine.Id = snackMachineInGrain.Id;
        snackMachine.CreatedAt = snackMachineInGrain.CreatedAt;
        snackMachine.LastModifiedAt = snackMachineInGrain.LastModifiedAt;
        snackMachine.DeletedAt = snackMachineInGrain.DeletedAt;
        snackMachine.CreatedBy = snackMachineInGrain.CreatedBy;
        snackMachine.LastModifiedBy = snackMachineInGrain.LastModifiedBy;
        snackMachine.DeletedBy = snackMachineInGrain.DeletedBy;
        snackMachine.IsDeleted = snackMachineInGrain.IsDeleted;
        snackMachine.MoneyInside = snackMachineInGrain.MoneyInside.ToProjection(snackMachine.MoneyInside);
        snackMachine.AmountInTransaction = snackMachineInGrain.AmountInTransaction;
        snackMachine.Slots = await Task.WhenAll(snackMachineInGrain.Slots.Select(slot => slot.ToProjection(getSnackNamePicture, snackMachine.Slots.FirstOrDefault(sl => sl.MachineId == slot.MachineId && sl.Position == slot.Position))));
        snackMachine.SlotsCount = snackMachineInGrain.SlotsCount;
        snackMachine.SnackCount = snackMachineInGrain.SnackCount;
        snackMachine.SnackQuantity = snackMachineInGrain.SnackQuantity;
        snackMachine.SnackAmount = snackMachineInGrain.SnackAmount;
        return snackMachine;
    }
}
