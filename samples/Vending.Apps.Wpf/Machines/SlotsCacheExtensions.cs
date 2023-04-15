using System.Collections.ObjectModel;
using DynamicData;
using Vending.App.Wpf.Snacks;
using Vending.Domain.Abstractions.Machines;

namespace Vending.App.Wpf.Machines;

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

    public static void LoadWith(this SourceCache<SlotEditViewModel, int> sourceCache, IList<MachineSlot> slots, ReadOnlyObservableCollection<SnackViewModel> snacks)
    {
        sourceCache.Edit(updater => updater.Load(slots.Select(slot => new SlotEditViewModel(slot, snacks))));
    }
}