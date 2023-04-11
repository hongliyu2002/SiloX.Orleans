using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace Vending.App.Machines;

public partial class MachineEditWindow
{
    public MachineEditWindow()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
                           {
                               this.OneWayBind(ViewModel, vm => vm.Id, v => v.IdTextBox.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyYuan1, v => v.MoneyYuan1TextBox.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyYuan2, v => v.MoneyYuan2TextBox.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyYuan5, v => v.MoneyYuan5TextBox.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyYuan10, v => v.MoneyYuan10TextBox.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyYuan20, v => v.MoneyYuan20TextBox.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyYuan50, v => v.MoneyYuan50TextBox.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyYuan100, v => v.MoneyYuan100TextBox.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.MoneyAmount, v => v.MoneyAmountText.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.CurrentSlot, v => v.SlotsListBox.SelectedItem).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.Slots, v => v.SlotsListBox.ItemsSource).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.IsDeletedCheckBox.IsChecked).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.MoneyYuan1TextBox.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.MoneyYuan2TextBox.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.MoneyYuan5TextBox.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.MoneyYuan10TextBox.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.MoneyYuan20TextBox.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.MoneyYuan50TextBox.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.MoneyYuan100TextBox.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.SlotsListBox.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.AddSlotButton.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.RemoveSlotButton.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.AddSlotCommand, v => v.AddSlotButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.RemoveSlotCommand, v => v.RemoveSlotButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.SaveMachineCommand, v => v.SaveMachineButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.CloseCommand, v => v.CloseButton).DisposeWith(disposable);
                               ViewModel?.ConfirmRemoveSlot.RegisterHandler(ShowMessageBox).DisposeWith(disposable);
                           });
    }

    private void ShowMessageBox(InteractionContext<string, bool> interaction)
    {
        var result = MessageBox.Show($"Are you sure you want to remove {interaction.Input}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        interaction.SetOutput(result == MessageBoxResult.Yes);
    }
}