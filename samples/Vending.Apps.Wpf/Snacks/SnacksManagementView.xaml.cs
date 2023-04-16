using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using Fluxera.Utilities.Extensions;
using Orleans.FluentResults;
using ReactiveUI;
using SiloX.Domain.Abstractions.Extensions;

namespace Vending.Apps.Wpf.Snacks;

public partial class SnacksManagementView
{
    public SnacksManagementView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
                           {
                               this.OneWayBind(ViewModel, vm => vm.ErrorInfo, v => v.ErrorLabel.Visibility, StringToVisibilityConverter).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.ErrorInfo, v => v.ErrorLabel.Content).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.NavigationSide, v => v.NavigationGridGridColumn, NavigationSideToIntConverter).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.SearchTerm, v => v.SearchTextBox.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.Snacks, v => v.SnackItemsListBox.ItemsSource).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.CurrentSnack, v => v.SnackItemsListBox.SelectedItem).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.CurrentSnackEdit, v => v.EditViewHost.ViewModel).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.AddSnackCommand, v => v.AddSnackButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.RemoveSnackCommand, v => v.RemoveSnackButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.MoveNavigationSideCommand, v => v.MoveNavigationSideButton).DisposeWith(disposable);
                               ViewModel?.ConfirmRemoveSnackInteraction.RegisterHandler(ConfirmRemoveSnack).DisposeWith(disposable);
                               ViewModel?.ErrorsInteraction.RegisterHandler(HandleErrors).DisposeWith(disposable);
                           });
    }

    private Visibility StringToVisibilityConverter(string value)
    {
        return value.IsNotNullOrEmpty() ? Visibility.Visible : Visibility.Collapsed;
    }

    private int NavigationSideToIntConverter(NavigationSide side)
    {
        return side == NavigationSide.Left ? 0 : 2;
    }

    private void ConfirmRemoveSnack(InteractionContext<string, bool> interaction)
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

    public int NavigationGridGridColumn
    {
        get => (int)NavigationGrid.GetValue(Grid.ColumnProperty);
        set => NavigationGrid.SetValue(Grid.ColumnProperty, value);
    }
}