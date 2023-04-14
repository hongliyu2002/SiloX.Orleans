using System.Globalization;
using System.Reactive;
using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace Vending.App.Wpf.Machines;

public partial class MachinesManagementView
{
    public MachinesManagementView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
                           {
                               this.Bind(ViewModel, vm => vm.PageSize, v => v.PageSizeTextBox.Text, IntToTextConverter, TextToIntConverter).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyAmountStart, v => v.MoneyAmountStartTextBox.Text, NullableDecimalToTextConverter, TextToNullableDecimalConverter).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyAmountEnd, v => v.MoneyAmountEndTextBox.Text, NullableDecimalToTextConverter, TextToNullableDecimalConverter).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.Machines, v => v.MachineItemsDataGrid.ItemsSource).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.CurrentMachine, v => v.MachineItemsDataGrid.SelectedItem).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.PageNumber, v => v.PageNumberText.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.PageCount, v => v.PageCountText.Text).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.AddMachineCommand, v => v.AddMachineButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.EditMachineCommand, v => v.EditMachineButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.RemoveMachineCommand, v => v.RemoveMachineButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.SyncMachinesCommand, v => v.SyncMachinesButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.GoPreviousPageCommand, v => v.PreviousPageButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.GoNextPageCommand, v => v.NextPageButton).DisposeWith(disposable);
                               ViewModel?.ShowEditMachine.RegisterHandler(ShowMachineEditWindow).DisposeWith(disposable);
                               ViewModel?.ConfirmRemoveMachine.RegisterHandler(ShowMessageBox).DisposeWith(disposable);
                           });
    }

    private string IntToTextConverter(int number)
    {
        return number.ToString();
    }

    private int TextToIntConverter(string text)
    {
        return int.TryParse(text, out var number) ? number : 0;
    }

    private string NullableDecimalToTextConverter(decimal? amount)
    {
        return amount.HasValue ? amount.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
    }

    private decimal? TextToNullableDecimalConverter(string text)
    {
        if (decimal.TryParse(text, out var amount))
        {
            return amount;
        }
        return null;
    }

    private void ShowMachineEditWindow(InteractionContext<MachineEditWindowModel, Unit> interaction)
    {
        var window = new MachineEditWindow { ViewModel = interaction.Input };
        var hostWindow = this.GetParentWindow();
        if (hostWindow != null)
        {
            window.Owner = hostWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
        window.ShowDialog();
        interaction.SetOutput(Unit.Default);
    }

    private void ShowMessageBox(InteractionContext<string, bool> interaction)
    {
        var result = MessageBox.Show($"Are you sure you want to remove {interaction.Input}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        interaction.SetOutput(result == MessageBoxResult.Yes);
    }
}