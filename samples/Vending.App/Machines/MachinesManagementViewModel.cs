using Orleans;
using ReactiveUI;

namespace Vending.App.Machines;

public class MachinesManagementViewModel : ReactiveObject, IHasClusterClient
{
    /// <inheritdoc />
    public MachinesManagementViewModel()
    {
    }

    /// <inheritdoc />
    public IClusterClient? ClusterClient { get; set; }
}