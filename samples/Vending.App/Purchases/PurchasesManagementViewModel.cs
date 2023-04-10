using Orleans;
using ReactiveUI;

namespace Vending.App.Purchases;

public class PurchasesManagementViewModel : ReactiveObject, IOrleansObject
{

    /// <inheritdoc />
    public PurchasesManagementViewModel()
    {
    }

    /// <inheritdoc />
    public IClusterClient? ClusterClient { get; set; }
}