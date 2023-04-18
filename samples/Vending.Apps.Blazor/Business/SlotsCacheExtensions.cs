using DynamicData;
using Vending.Domain.Abstractions.Machines;

namespace Vending.Apps.Blazor.Business;

public static class SlotsCacheExtensions
{
    public static void AddOrUpdateWith(this SourceCache<SlotViewModel, int> sourceCache, MachineSlot slot)
    {
        sourceCache.Edit(updater =>
                         {
                             var current = updater.Lookup(slot.Position);
                             if (current.HasValue)
                             {
                                 current.Value.UpdateWith(slot);
                             }
                             else
                             {
                                 updater.AddOrUpdate(new SlotViewModel(slot));
                             }
                         });
    }

    public static void AddOrUpdateWith(this SourceCache<SlotViewModel, int> sourceCache, IList<MachineSlot> slots)
    {
        sourceCache.Edit(updater =>
                         {
                             foreach (var slot in slots)
                             {
                                 var current = updater.Lookup(slot.Position);
                                 if (current.HasValue)
                                 {
                                     current.Value.UpdateWith(slot);
                                 }
                                 else
                                 {
                                     updater.AddOrUpdate(new SlotViewModel(slot));
                                 }
                             }
                         });
    }

    public static void RemoveWith(this SourceCache<SlotViewModel, int> sourceCache, int position)
    {
        sourceCache.Edit(updater => updater.Remove(position));
    }

    public static void RemoveWith(this SourceCache<SlotViewModel, int> sourceCache, IList<int> positions)
    {
        sourceCache.Edit(updater => updater.RemoveKeys(positions));
    }

    public static void RemoveWith(this SourceCache<SlotViewModel, int> sourceCache, MachineSlot slot)
    {
        sourceCache.Edit(updater => updater.Remove(slot.Position));
    }

    public static void RemoveWith(this SourceCache<SlotViewModel, int> sourceCache, IList<MachineSlot> slots)
    {
        sourceCache.Edit(updater => updater.RemoveKeys(slots.Select(slot => slot.Position)));
    }

    public static void LoadWith(this SourceCache<SlotViewModel, int> sourceCache, IList<MachineSlot> slots)
    {
        sourceCache.Edit(updater => updater.Load(slots.Select(slot => new SlotViewModel(slot))));
    }
}