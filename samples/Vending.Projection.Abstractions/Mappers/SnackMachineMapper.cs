using Vending.Projection.Abstractions.Entities;

namespace Vending.Projection.Abstractions.Mappers;

public static class SnackMachineMapper
{
    public static SnackMachine ToProjection(this Domain.Abstractions.States.SnackMachine source, SnackMachine? snackMachine = null)
    {
        snackMachine ??= new SnackMachine();
        snackMachine.Id = source.Id;
        snackMachine.CreatedAt = source.CreatedAt;
        snackMachine.LastModifiedAt = source.LastModifiedAt;
        snackMachine.DeletedAt = source.DeletedAt;
        snackMachine.CreatedBy = source.CreatedBy;
        snackMachine.LastModifiedBy = source.LastModifiedBy;
        snackMachine.DeletedBy = source.DeletedBy;
        snackMachine.IsDeleted = source.IsDeleted;
        snackMachine.MoneyInside = source.MoneyInside.ToProjection(snackMachine.MoneyInside);
        snackMachine.AmountInTransaction = source.AmountInTransaction;
        snackMachine.Slots = source.Slots.Select(slot => slot.ToProjection(snackMachine.Slots.FirstOrDefault(sl => sl.MachineId == slot.MachineId && sl.Position == slot.Position))).ToList();
        snackMachine.SlotsCount = source.SlotsCount;
        snackMachine.SnackCount = source.SnackCount;
        snackMachine.SnackQuantity = source.SnackQuantity;
        snackMachine.SnackAmount = source.SnackAmount;
        return snackMachine;
    }

    public static Domain.Abstractions.States.SnackMachine ToDomain(this SnackMachine source, Domain.Abstractions.States.SnackMachine? snackMachine = null)
    {
        snackMachine ??= new Domain.Abstractions.States.SnackMachine();
        snackMachine.Id = source.Id;
        snackMachine.CreatedAt = source.CreatedAt;
        snackMachine.LastModifiedAt = source.LastModifiedAt;
        snackMachine.DeletedAt = source.DeletedAt;
        snackMachine.CreatedBy = source.CreatedBy;
        snackMachine.LastModifiedBy = source.LastModifiedBy;
        snackMachine.DeletedBy = source.DeletedBy;
        snackMachine.IsDeleted = source.IsDeleted;
        snackMachine.MoneyInside = source.MoneyInside.ToDomain();
        snackMachine.AmountInTransaction = source.AmountInTransaction;
        snackMachine.Slots = source.Slots.Select(slot => slot.ToDomain(snackMachine.Slots.FirstOrDefault(sl => sl.MachineId == slot.MachineId && sl.Position == slot.Position))).ToList();
        return snackMachine;
    }
}
