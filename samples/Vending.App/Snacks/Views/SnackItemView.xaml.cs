using System.Reactive.Disposables;
using System.Windows.Media.Imaging;
using ReactiveUI;

namespace Vending.App.Views;

public partial class SnackItemView
{
    public SnackItemView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
                           {
                               this.OneWayBind(ViewModel, vm => vm.Name, v => v.NameRun.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.PictureUrl, v => v.PictureImage.Source, uri => new BitmapImage(uri)).DisposeWith(disposable);
                           });
    }
}
