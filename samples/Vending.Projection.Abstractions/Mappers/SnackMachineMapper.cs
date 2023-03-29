using Vending.Projection.Abstractions.Entities;

namespace Vending.Projection.Abstractions.Mappers;

public static class SnackMachineMapper
{
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
