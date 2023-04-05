using Vending.App.ViewModels;

namespace Vending.App.Views;

public partial class SnacksManagementView
{
    public SnacksManagementView()
    {
        InitializeComponent();
        ViewModel = new SnacksManagementViewModel();
    }
}
