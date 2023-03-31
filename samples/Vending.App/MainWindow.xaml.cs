using System.Windows;
using System.Windows.Controls;
using Fluxera.Extensions.Hosting;
using Orleans;
using Vending.App.Controls;

namespace Vending.App;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IMainWindow
{
    private readonly IClusterClient _clusterClient;
    private readonly UserControl _snacksManagementControl;
    private readonly UserControl _machinesManagementControl;

    public MainWindow(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
        _snacksManagementControl = new SnacksManagementControl(clusterClient);
        _machinesManagementControl = new MachinesManagementControl(clusterClient);
        InitializeComponent();
    }

    private void SnacksManagementButton_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = _snacksManagementControl;
    }

    private void MachinesManagementButton_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = _machinesManagementControl;
    }
}
