using System.Reactive.Disposables;
using Fluxera.Extensions.Hosting;
using Orleans;
using ReactiveUI;
using Splat;
using Vending.App.ViewModels;

namespace Vending.App;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : IMainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new MainViewModel();
        var clusterClient = Locator.Current.GetService<IClusterClient>();
        this.WhenActivated(disposable =>
                           {
                               this.OneWayBind(ViewModel, vm => vm.SelectedViewModel, v => v.MainContent.Content).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.ManageSnacksCommand, v => v.SnacksManagementMenuItem).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.ManageSnackMachinesCommand, v => v.MachinesManagementMenuItem).DisposeWith(disposable);
                           });
    }
}
