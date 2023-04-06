using System.Reactive.Disposables;
using Fluxera.Extensions.Hosting;
using ReactiveUI;
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
        this.WhenActivated(disposable =>
                           {
                               this.OneWayBind(ViewModel, vm => vm.SelectedViewModel, v => v.MainViewHost.ViewModel).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.ManageSnacksCommand, v => v.SnacksManagementMenuItem).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.ManageSnackMachinesCommand, v => v.MachinesManagementMenuItem).DisposeWith(disposable);
                           });
    }
}
