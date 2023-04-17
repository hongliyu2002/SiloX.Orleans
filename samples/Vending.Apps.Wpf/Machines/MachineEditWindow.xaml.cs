using System.Reactive;
using System.Reactive.Disposables;
using System.Windows;
using Fluxera.Utilities.Extensions;
using Orleans.FluentResults;
using ReactiveUI;
using SiloX.Domain.Abstractions.Extensions;

namespace Vending.Apps.Wpf.Machines;

public partial class MachineEditWindow
{
    public MachineEditWindow()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
                           {
                               this.OneWayBind(ViewModel, vm => vm.ErrorInfo, v => v.ErrorLabel.Visibility, StringToVisibilityConverter).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.ErrorInfo, v => v.ErrorLabel.Content).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.Id, v => v.IdTextBox.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyYuan1, v => v.MoneyYuan1TextBox.Text, IntToTextConverter, TextToIntConverter).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyYuan2, v => v.MoneyYuan2TextBox.Text, IntToTextConverter, TextToIntConverter).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyYuan5, v => v.MoneyYuan5TextBox.Text, IntToTextConverter, TextToIntConverter).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyYuan10, v => v.MoneyYuan10TextBox.Text, IntToTextConverter, TextToIntConverter).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyYuan20, v => v.MoneyYuan20TextBox.Text, IntToTextConverter, TextToIntConverter).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyYuan50, v => v.MoneyYuan50TextBox.Text, IntToTextConverter, TextToIntConverter).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.MoneyYuan100, v => v.MoneyYuan100TextBox.Text, IntToTextConverter, TextToIntConverter).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.MoneyAmount, v => v.MoneyAmountText.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.Slots, v => v.SlotsListBox.ItemsSource).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.CurrentSlot, v => v.SlotsListBox.SelectedItem).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.IsDeletedCheckBox.IsChecked).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.MoneyYuan1TextBox.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.MoneyYuan2TextBox.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.MoneyYuan5TextBox.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.MoneyYuan10TextBox.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.MoneyYuan20TextBox.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.MoneyYuan50TextBox.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.MoneyYuan100TextBox.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.AddSlotCommand, v => v.AddSlotButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.RemoveSlotCommand, v => v.RemoveSlotButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.SaveMachineCommand, v => v.SaveMachineButton).DisposeWith(disposable);
                               ViewModel?.ConfirmRemoveSlotInteraction.RegisterHandler(ConfirmRemoveSlot).DisposeWith(disposable);
                               ViewModel?.NotifySavedMachineInteraction.RegisterHandler(NotifySavedMachine).DisposeWith(disposable);
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

    private void ConfirmRemoveSlot(InteractionContext<string, bool> interaction)
    {
        var result = MessageBox.Show(this, $"Are you sure you want to remove {interaction.Input}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        interaction.SetOutput(result == MessageBoxResult.Yes);
    }

    private void NotifySavedMachine(InteractionContext<string, Unit> interaction)
    {
        MessageBox.Show(this, interaction.Input, "Notification", MessageBoxButton.YesNo, MessageBoxImage.Question);
        interaction.SetOutput(Unit.Default);
    }

    private void HandleErrors(InteractionContext<IEnumerable<IError>, ErrorRecovery> interaction)
    {
        var errors = interaction.Input;
        var message = errors.ToMessage();
        var result = MessageBox.Show(this, $"{message}.\n\nRetry or cancel?", "Errors occurred when operating", MessageBoxButton.OKCancel, MessageBoxImage.Error);
        interaction.SetOutput(result == MessageBoxResult.OK ? ErrorRecovery.Retry : ErrorRecovery.Abort);
    }
}