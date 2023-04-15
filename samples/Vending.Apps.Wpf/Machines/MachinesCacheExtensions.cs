using DynamicData;
using Vending.Projection.Abstractions.Machines;

namespace Vending.Apps.Wpf.Machines;

public static class MachinesCacheExtensions
{
    public static void AddOrUpdateWith(this SourceCache<MachineViewModel, Guid> sourceCache, MachineInfo machine)
    {
        sourceCache.Edit(updater =>
                         {
                             var current = updater.Lookup(machine.Id);
                             if (current.HasValue)
                             {
                                 current.Value.UpdateWith(machine);
                             }
                             else
                             {
                                 updater.AddOrUpdate(new MachineViewModel(machine));
                             }
                         });
    }

    public static void AddOrUpdateWith(this SourceCache<MachineViewModel, Guid> sourceCache, IList<MachineInfo> machines)
    {
        sourceCache.Edit(updater =>
                         {
                             foreach (var machine in machines)
                             {
                                 var current = updater.Lookup(machine.Id);
                                 if (current.HasValue)
                                 {
                                     current.Value.UpdateWith(machine);
                                 }
                                 else
                                 {
                                     updater.AddOrUpdate(new MachineViewModel(machine));
                                 }
                             }
                         });
    }
}