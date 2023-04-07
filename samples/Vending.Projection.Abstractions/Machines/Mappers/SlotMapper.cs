using Vending.Domain.Abstractions.Machines;

namespace Vending.Projection.Abstractions.Machines;

public static class SlotMapper
{
    /// <summary>
    ///     Maps a <see cref="Domain.Abstractions.Machines.Slot" /> to a <see cref="SlotInfo" />.
    /// </summary>
    /// <param name="slot">The <see cref="Domain.Abstractions.Machines.Slot" /> to map. </param>
    /// <param name="getSnackNamePicture">A function that returns the name and picture of a snack.</param>
    /// <param name="slotInfo">The <see cref="SlotInfo" /> to map to. If <c>null</c>, a new <see cref="SlotInfo" /> will be created.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    public static async Task<SlotInfo> ToProjection(this Slot slot, Func<Guid, Task<(string, string?)>> getSnackNamePicture, SlotInfo? slotInfo = null)
    {
        slotInfo ??= new SlotInfo();
        slotInfo.MachineId = slot.MachineId;
        slotInfo.Position = slot.Position;
        if (slot.SnackPile != null)
        {
            slotInfo.SnackPile = await slot.SnackPile.ToProjection(getSnackNamePicture, slotInfo.SnackPile);
        }
        return slotInfo;
    }
}
