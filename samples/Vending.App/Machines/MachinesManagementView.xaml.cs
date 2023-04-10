using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace Vending.App.Machines;

public partial class MachinesManagementView
{
    public MachinesManagementView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
                           {
                               this.Bind(ViewModel, vm => vm.PageSize, v => v.PageSizeTextBox.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyAmountStart, v => v.MoneyAmountStartTextBox.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyAmountEnd, v => v.MoneyAmountEndTextBox.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.CurrentMachineItem, v => v.MachineItemsDataGrid.SelectedItem).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.MachineItems, v => v.MachineItemsDataGrid.ItemsSource).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.PageNumber, v => v.CurrentPageText.Text).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.AddMachineCommand, v => v.AddMachineButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.RemoveMachineCommand, v => v.RemoveMachineButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.GoPreviousPageCommand, v => v.PreviousPageButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.GoNextPageCommand, v => v.NextPageButton).DisposeWith(disposable);
                               ViewModel?.ConfirmRemoveMachine.RegisterHandler(ShowMessageBox).DisposeWith(disposable);
                           });
    }
    
    private void ShowMessageBox(InteractionContext<string, bool> confirmRemoveMachine)
    {
        var result = MessageBox.Show($"Are you sure you want to remove {confirmRemoveMachine.Input}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        confirmRemoveMachine.SetOutput(result == MessageBoxResult.Yes);
    }
}