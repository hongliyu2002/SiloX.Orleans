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
                               this.OneWayBind(ViewModel, vm => vm.CurrentViewModel, v => v.MainViewHost.ViewModel).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.GoSnacksManagementCommand, v => v.SnacksManagementMenuItem).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.GoMachinesManagementCommand, v => v.MachinesManagementMenuItem).DisposeWith(disposable);
                           });
    }
}
