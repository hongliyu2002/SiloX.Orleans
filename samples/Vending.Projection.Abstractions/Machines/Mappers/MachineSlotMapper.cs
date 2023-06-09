﻿using Vending.Domain.Abstractions.Machines;

namespace Vending.Projection.Abstractions.Machines;

public static class MachineSlotMapper
{
    /// <summary>
    ///     Maps a <see cref="MachineSlot" /> to a <see cref="MachineSlotInfo" />.
    /// </summary>
    /// <param name="slot">The <see cref="MachineSlot" /> to map. </param>
    /// <param name="getSnackNamePicture">A function that returns the name and picture of a snack.</param>
    /// <param name="slotInfo">The <see cref="MachineSlotInfo" /> to map to. If <c>null</c>, a new <see cref="MachineSlotInfo" /> will be created.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    public static async Task<MachineSlotInfo> ToProjection(this MachineSlot slot, Func<Guid, Task<(string, string?)>> getSnackNamePicture, MachineSlotInfo? slotInfo = null)
    {
        slotInfo ??= new MachineSlotInfo();
        slotInfo.MachineId = slot.MachineId;
        slotInfo.Position = slot.Position;
        if (slot.SnackPile != null)
        {
            slotInfo.SnackPile = await slot.SnackPile.ToProjection(getSnackNamePicture, slotInfo.SnackPile);
        }
        return slotInfo;
    }

    /// <summary>
    ///     Maps a <see cref="MachineSlotInfo" /> to a <see cref="MachineSlot" />.
    /// </summary>
    /// <param name="slotInfo">The <see cref="MachineSlotInfo" /> to map. </param>
    /// <param name="slot">The <see cref="MachineSlot" /> to map to. If <c>null</c>, a new <see cref="MachineSlot" /> will be created.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    public static MachineSlot ToDomain(this MachineSlotInfo slotInfo, MachineSlot? slot = null)
    {
        slot ??= new MachineSlot(slotInfo.MachineId, slotInfo.Position);
        if (slotInfo.SnackPile != null)
        {
            slot.SnackPile = slotInfo.SnackPile.ToDomain(slot.SnackPile);
        }
        return slot;
    }
}