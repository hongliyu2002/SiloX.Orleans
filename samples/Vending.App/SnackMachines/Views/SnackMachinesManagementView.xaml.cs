using Vending.App.ViewModels;

namespace Vending.App.Views;

public partial class SnackMachinesManagementView
{
    public SnackMachinesManagementView()
    {
        InitializeComponent();
        ViewModel = new SnackMachinesManagementViewModel();
    }
}
