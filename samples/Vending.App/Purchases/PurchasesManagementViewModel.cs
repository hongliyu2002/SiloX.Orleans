using Orleans;
using ReactiveUI;

namespace Vending.App.Purchases;

public class PurchasesManagementViewModel : ReactiveObject, IHasClusterClient
{

    /// <inheritdoc />
    public PurchasesManagementViewModel()
    {
    }

    /// <inheritdoc />
    public IClusterClient? ClusterClient { get; set; }
}