using Orleans;
using ReactiveUI;

namespace Vending.App.Wpf.Purchases;

public class PurchasesManagementViewModel : ReactiveObject, IOrleansObject
{

    /// <inheritdoc />
    public PurchasesManagementViewModel()
    {
    }

    /// <inheritdoc />
    public IClusterClient? ClusterClient { get; set; }
}