using Vending.Projection.Abstractions.Entities;

namespace Vending.Projection.Abstractions.Mappers;

public static class SlotMapper
{
    /// <summary>
    ///     Maps a <see cref="Domain.Abstractions.States.Slot" /> to a <see cref="Slot" />.
    /// </summary>
    /// <param name="slotInGrain">The <see cref="Domain.Abstractions.States.Slot" /> to map. </param>
    /// <param name="getSnackNamePicture">A function that returns the name and picture of a snack.</param>
    /// <param name="slot">The <see cref="Slot" /> to map to. If <c>null</c>, a new <see cref="Slot" /> will be created.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
