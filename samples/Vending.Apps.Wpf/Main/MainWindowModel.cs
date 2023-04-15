using System.Reactive;
using ReactiveUI;
using Vending.Apps.Wpf.Machines;
using Vending.Apps.Wpf.Purchases;
using Vending.Apps.Wpf.Snacks;

namespace Vending.Apps.Wpf;

public class MainWindowModel : ReactiveObject
{
    private readonly Dictionary<string, ReactiveObject> _viewModels;

    public MainWindowModel()
    {
        _viewModels = new Dictionary<string, ReactiveObject>
                      {
                          { "Snacks", new SnacksManagementViewModel() },
                          { "Machines", new MachinesManagementViewModel() },
                          { "Purchases", new PurchasesManagementViewModel() }
                      };
        CurrentViewModel ??= _viewModels["Snacks"];
        // Create commands
        GoSnacksManagementCommand = ReactiveCommand.Create(GoSnacksManagement);
        GoMachinesManagementCommand = ReactiveCommand.Create(GoMachinesManagement);
        GoPurchasesManagementCommand = ReactiveCommand.Create(GoPurchasesManagement);
    }

    #region Properties

    private ReactiveObject? _currentViewModel;
    public ReactiveObject? CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Unit> GoSnacksManagementCommand { get; }

    private void GoSnacksManagement()
    {
        CurrentViewModel = _viewModels["Snacks"];
    }

    public ReactiveCommand<Unit, Unit> GoMachinesManagementCommand { get; }

    private void GoMachinesManagement()
    {
        CurrentViewModel = _viewModels["Machines"];
    }

    public ReactiveCommand<Unit, Unit> GoPurchasesManagementCommand { get; }

    private void GoPurchasesManagement()
    {
        CurrentViewModel = _viewModels["Purchases"];
    }

    #endregion

}