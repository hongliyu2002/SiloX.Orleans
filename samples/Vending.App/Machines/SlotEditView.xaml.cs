using System.Reactive.Disposables;
using ReactiveUI;

namespace Vending.App.Machines;

public partial class SlotEditView
{
    public SlotEditView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
                           {
                               this.Bind(ViewModel, vm => vm.Position, v => v.PositionTextBox.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.Quantity, v => v.QuantityTextBox.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.Price, v => v.PriceTextBox.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.CurrentSnack, v => v.SnackItemsComboBox.SelectedItem).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.Snacks, v => v.SnackItemsComboBox.ItemsSource).DisposeWith(disposable);
                           });
    }
}