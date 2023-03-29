using Vending.Projection.Abstractions.Entities;

namespace Vending.Projection.Abstractions.Mappers;

public static class SlotMapper
{
    public static async Task<Slot> ToProjection(this Domain.Abstractions.States.Slot slotInGrain, Func<Guid, Task<(string, string?)>> getSnackNamePicture, Slot? slot = null)
    {
        slot ??= new Slot();
        slot.MachineId = slotInGrain.MachineId;
        slot.Position = slotInGrain.Position;
        if (slotInGrain.SnackPile != null)
        {
            slot.SnackPile = await slotInGrain.SnackPile.ToProjection(getSnackNamePicture, slot.SnackPile);
        }
        return slot;
    }
}
