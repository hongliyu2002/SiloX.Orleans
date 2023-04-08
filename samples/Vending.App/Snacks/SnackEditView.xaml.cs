using System.Reactive.Disposables;
using System.Windows.Media.Imaging;
using ReactiveUI;

namespace Vending.App.Snacks;

public partial class SnackEditView
{
    public SnackEditView()
    {
        InitializeComponent();
        // this.WhenActivated(disposable =>
        //                    {
        //                        this.OneWayBind(ViewModel, vm => vm.Id, v => v.IdTextBox.Text).DisposeWith(disposable);
        //                        this.Bind(ViewModel, vm => vm.Name, v => v.NameTextBox.Text).DisposeWith(disposable);
        //                        this.Bind(ViewModel, vm => vm.PictureUrl, v => v.PictureTextBox.Text).DisposeWith(disposable);
        //                        this.OneWayBind(ViewModel, vm => vm.PictureUrl, v => v.PictureImage.Source, uri => new BitmapImage(uri)).DisposeWith(disposable);
        //                        this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.IsDeletedCheckBox.IsChecked).DisposeWith(disposable);
        //                        this.BindCommand(ViewModel, vm => vm.S, v => v.AddSnackButton).DisposeWith(disposable);
        //                        this.BindCommand(ViewModel, vm => vm.RemoveSnackCommand, v => v.RemoveSnackButton).DisposeWith(disposable);
        //
        //                    });

    }
}
