using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;

namespace Vending.App.Snacks;

public partial class SnacksManagementView
{
    public SnacksManagementView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
                           {
                               this.Bind(ViewModel, vm => vm.SearchTerm, v => v.SearchTextBox.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.CurrentSnack, v => v.SnackItemsListBox.SelectedItem).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.CurrentSnackEdit, v => v.EditViewHost.ViewModel).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.Snacks, v => v.SnackItemsListBox.ItemsSource).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.NavigationSide, v => v.NavigationGridGridColumn, side => side == NavigationSide.Left ? 0 : 2).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.AddSnackCommand, v => v.AddSnackButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.RemoveSnackCommand, v => v.RemoveSnackButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.MoveNavigationSideCommand, v => v.MoveNavigationSideButton).DisposeWith(disposable);
                               ViewModel?.ConfirmRemoveSnack.RegisterHandler(ShowMessageBox).DisposeWith(disposable);
                           });
    }

    private void ShowMessageBox(InteractionContext<string, bool> confirmRemoveSnack)
    {
        var result = MessageBox.Show($"Are you sure you want to remove {confirmRemoveSnack.Input}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        confirmRemoveSnack.SetOutput(result == MessageBoxResult.Yes);
    }

    public int NavigationGridGridColumn
    {
        get => (int)NavigationGrid.GetValue(Grid.ColumnProperty);
        set => NavigationGrid.SetValue(Grid.ColumnProperty, value);
    }
}