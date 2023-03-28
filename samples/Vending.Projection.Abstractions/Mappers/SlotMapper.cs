using Vending.Projection.Abstractions.Entities;

namespace Vending.Projection.Abstractions.Mappers;

public static class SlotMapper
{
    public static Slot ToProjection(this Domain.Abstractions.States.Slot source, Slot? slot = null)
    {
        slot ??= new Slot();
        slot.MachineId = source.MachineId;
        slot.Position = source.Position;
        slot.SnackPile = source.SnackPile?.ToProjection(slot.SnackPile);
        return slot;
    }

    public static Domain.Abstractions.States.Slot ToDomain(this Slot source, Domain.Abstractions.States.Slot? slot = null)
    {
        slot ??= new Domain.Abstractions.States.Slot();
        slot.MachineId = source.MachineId;
        slot.Position = source.Position;
        slot.SnackPile = source.SnackPile?.ToDomain();
        return slot;
    }
}
