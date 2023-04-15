using System.Collections.ObjectModel;
using DynamicData;
using Vending.Apps.Wpf.Snacks;
using Vending.Domain.Abstractions.Machines;

namespace Vending.Apps.Wpf.Machines;

public static class SlotsCacheExtensions
{
    public static void AddOrUpdateWith(this SourceCache<SlotEditViewModel, int> sourceCache, MachineSlot slot, ReadOnlyObservableCollection<SnackViewModel> snacks)
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
                                 updater.AddOrUpdate(new SlotEditViewModel(slot, snacks));
                             }
                         });
    }

    public static void AddOrUpdateWith(this SourceCache<SlotEditViewModel, int> sourceCache, IList<MachineSlot> slots, ReadOnlyObservableCollection<SnackViewModel> snacks)
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
                                     updater.AddOrUpdate(new SlotEditViewModel(slot, snacks));
                                 }
                             }
                         });
    }

    public static void RemoveWith(this SourceCache<SlotEditViewModel, int> sourceCache, int position)
    {
        sourceCache.Edit(updater => updater.Remove(position));
    }

    public static void RemoveWith(this SourceCache<SlotEditViewModel, int> sourceCache, IList<int> positions)
    {
        sourceCache.Edit(updater => updater.RemoveKeys(positions));
    }

    public static void RemoveWith(this SourceCache<SlotEditViewModel, int> sourceCache, MachineSlot slot)
    {
        sourceCache.Edit(updater => updater.Remove(slot.Position));
    }

    public static void RemoveWith(this SourceCache<SlotEditViewModel, int> sourceCache, IList<MachineSlot> slots)
    {
        sourceCache.Edit(updater => updater.RemoveKeys(slots.Select(slot => slot.Position)));
    }

    public static void LoadWith(this SourceCache<SlotEditViewModel, int> sourceCache, IList<MachineSlot> slots, ReadOnlyObservableCollection<SnackViewModel> snacks)
    {
        sourceCache.Edit(updater => updater.Load(slots.Select(slot => new SlotEditViewModel(slot, snacks))));
    }
}