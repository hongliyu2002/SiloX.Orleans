using DynamicData;
using Vending.Projection.Abstractions.Purchases;

namespace Vending.Apps.Blazor.Purchases;

public static class PurchasesCacheExtensions
{
    public static void AddOrUpdateWith(this SourceCache<PurchaseViewModel, Guid> sourceCache, PurchaseInfo purchase)
    {
        sourceCache.Edit(updater =>
                         {
                             var current = updater.Lookup(purchase.Id);
                             if (current.HasValue)
                             {
                                 current.Value.UpdateWith(purchase);
                             }
                             else
                             {
                                 updater.AddOrUpdate(new PurchaseViewModel(purchase));
                             }
                         });
    }

    public static void AddOrUpdateWith(this SourceCache<PurchaseViewModel, Guid> sourceCache, IList<PurchaseInfo> purchases)
    {
        sourceCache.Edit(updater =>
                         {
                             foreach (var purchase in purchases)
                             {
                                 var current = updater.Lookup(purchase.Id);
                                 if (current.HasValue)
                                 {
                                     current.Value.UpdateWith(purchase);
                                 }
                                 else
                                 {
                                     updater.AddOrUpdate(new PurchaseViewModel(purchase));
                                 }
                             }
                         });
    }
}