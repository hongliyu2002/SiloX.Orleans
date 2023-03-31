using System.Windows.Controls;
using Orleans;

namespace Vending.App.Controls;

public partial class MachinesManagementControl : UserControl
{
    private readonly IClusterClient _clusterClient;

    public MachinesManagementControl(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
        InitializeComponent();
    }
}

