﻿using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;

namespace Vending.App.Wpf.Snacks;

public partial class SnacksManagementView
{
    public SnacksManagementView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
                           {
                               this.OneWayBind(ViewModel, vm => vm.ErrorInfo, v => v.ErrorLabel.Content, NavigationSideToIntConverter).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.NavigationSide, v => v.NavigationGridGridColumn, NavigationSideToIntConverter).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.SearchTerm, v => v.SearchTextBox.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.Snacks, v => v.SnackItemsListBox.ItemsSource).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.CurrentSnack, v => v.SnackItemsListBox.SelectedItem).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.CurrentSnackEdit, v => v.EditViewHost.ViewModel).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.AddSnackCommand, v => v.AddSnackButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.RemoveSnackCommand, v => v.RemoveSnackButton).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.MoveNavigationSideCommand, v => v.MoveNavigationSideButton).DisposeWith(disposable);
                               ViewModel?.ConfirmRemoveSnack.RegisterHandler(ShowMessageBox).DisposeWith(disposable);
                           });
    }

    private int NavigationSideToIntConverter(NavigationSide side)
    {
        return side == NavigationSide.Left ? 0 : 2;
    }

    private void ShowMessageBox(InteractionContext<string, bool> interaction)
    {
        var result = MessageBox.Show($"Are you sure you want to remove {interaction.Input}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        interaction.SetOutput(result == MessageBoxResult.Yes);
    }

    public int NavigationGridGridColumn
    {
        get => (int)NavigationGrid.GetValue(Grid.ColumnProperty);
        set => NavigationGrid.SetValue(Grid.ColumnProperty, value);
    }
}