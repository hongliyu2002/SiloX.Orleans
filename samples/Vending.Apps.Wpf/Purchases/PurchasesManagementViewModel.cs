using ReactiveUI;

namespace Vending.Apps.Wpf.Purchases;

public class PurchasesManagementViewModel : ReactiveObject, IActivatableViewModel
{

    /// <inheritdoc />
    public PurchasesManagementViewModel()
    {
    }

    /// <inheritdoc />
    public ViewModelActivator Activator { get; } = new();

    private IClusterClient? _clusterClient;
    public IClusterClient? ClusterClient
    {
        get => _clusterClient;
        set => this.RaiseAndSetIfChanged(ref _clusterClient, value);
    }
}