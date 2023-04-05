using System.Reactive.Disposables;
using System.Windows.Controls;
using ReactiveUI;
using Vending.App.ViewModels;

namespace Vending.App.Views;

public partial class SnacksManagementView
{
    public SnacksManagementView()
    {
        InitializeComponent();
        ViewModel = new SnacksManagementViewModel();
        this.WhenActivated(disposable =>
                           {
                               this.OneWayBind(ViewModel, vm => vm.NavigationSide, v => v.NavigationGridGridColumn, side => side == NavigationSide.Left ? 0 : 2)
                                   .DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.AddSnackCommand, v => v.AddSnackButton)
                                   .DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.RemoveSnackCommand, v => v.RemoveSnackButton)
                                   .DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.MoveNavigationSideCommand, v => v.MoveNavigationSideButton)
                                   .DisposeWith(disposable);
                           });
    }

    public int NavigationGridGridColumn
    {
        get => (int)NavigationGrid.GetValue(Grid.ColumnProperty);
        set => NavigationGrid.SetValue(Grid.ColumnProperty, value);
    }

}
