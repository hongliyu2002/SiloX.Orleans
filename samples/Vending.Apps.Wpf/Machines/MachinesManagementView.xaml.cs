using System.Globalization;
using System.Reactive;
using System.Reactive.Disposables;
using System.Windows;
using Fluxera.Utilities.Extensions;
using Orleans.FluentResults;
using ReactiveUI;
using SiloX.Domain.Abstractions.Extensions;

namespace Vending.Apps.Wpf.Machines;

public partial class MachinesManagementView
{
    public MachinesManagementView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
                           {
                               this.OneWayBind(ViewModel, vm => vm.ErrorInfo, v => v.ErrorLabel.Visibility, StringToVisibilityConverter).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.ErrorInfo, v => v.ErrorLabel.Content).DisposeWith(disposable);
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
                               ViewModel?.ShowEditMachineInteraction.RegisterHandler(ShowEditMachine).DisposeWith(disposable);
                               ViewModel?.ConfirmRemoveMachineInteraction.RegisterHandler(ConfirmRemoveMachine).DisposeWith(disposable);
                               ViewModel?.ErrorsInteraction.RegisterHandler(HandleErrors).DisposeWith(disposable);
                           });
    }

    private Visibility StringToVisibilityConverter(string value)
    {
        return value.IsNotNullOrEmpty() ? Visibility.Visible : Visibility.Collapsed;
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

    private void ShowEditMachine(InteractionContext<MachineEditWindowModel, Unit> interaction)
    {
        var window = new MachineEditWindow { ViewModel = interaction.Input };
        var hostWindow = this.GetParentWindow();
        if (hostWindow != null)
        {
            window.Owner = hostWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
        window.Show();
        interaction.SetOutput(Unit.Default);
    }

    private void ConfirmRemoveMachine(InteractionContext<string, bool> interaction)
    {
        var result = MessageBox.Show($"Are you sure you want to remove {interaction.Input}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        interaction.SetOutput(result == MessageBoxResult.Yes);
    }

    private void HandleErrors(InteractionContext<IEnumerable<IError>, ErrorRecovery> interaction)
    {
        var errors = interaction.Input;
        var message = errors.ToMessage();
        var result = MessageBox.Show($"{message}.\n\nRetry or cancel?", "Errors occurred when operating", MessageBoxButton.OKCancel, MessageBoxImage.Error);
        interaction.SetOutput(result == MessageBoxResult.OK ? ErrorRecovery.Retry : ErrorRecovery.Abort);
    }
}