using DynamicData;
using Vending.Projection.Abstractions.Snacks;

namespace Vending.Apps.Wpf.Snacks;

public static class SnacksCacheExtensions
{
    public static void AddOrUpdateWith(this SourceCache<SnackViewModel, Guid> sourceCache, SnackInfo snack)
    {
        sourceCache.Edit(updater =>
                         {
                             var current = updater.Lookup(snack.Id);
                             if (current.HasValue)
                             {
                                 current.Value.UpdateWith(snack);
                             }
                             else
                             {
                                 updater.AddOrUpdate(new SnackViewModel(snack));
                             }
                         });
    }

    public static void AddOrUpdateWith(this SourceCache<SnackViewModel, Guid> sourceCache, IList<SnackInfo> snacks)
    {
        sourceCache.Edit(updater =>
                         {
                             foreach (var snack in snacks)
                             {
                                 var current = updater.Lookup(snack.Id);
                                 if (current.HasValue)
                                 {
                                     current.Value.UpdateWith(snack);
                                 }
                                 else
                                 {
                                     updater.AddOrUpdate(new SnackViewModel(snack));
                                 }
                             }
                         });
    }
}