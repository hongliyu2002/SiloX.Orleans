using System.Reactive.Disposables;
using System.Windows.Media.Imaging;
using ReactiveUI;

namespace Vending.App.Snacks;

public partial class SnackItemView
{
    public SnackItemView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
                           {
                               this.OneWayBind(ViewModel, vm => vm.Name, v => v.NameRun.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.PictureUrl, v => v.PictureImage.Source, uri => new BitmapImage(uri)).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.MachineCount, v => v.MachineCountRun.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.TotalQuantity, v => v.TotalQuantityRun.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.TotalAmount, v => v.TotalAmountRun.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.BoughtCount, v => v.BoughtCountRun.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.BoughtAmount, v => v.BoughtAmountRun.Text).DisposeWith(disposable);
                           });
    }
}