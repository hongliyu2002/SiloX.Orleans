using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Vending.App.ViewModels;

public class MainViewModel : ReactiveObject
{
    public MainViewModel()
    {
        this.WhenAnyValue(vm => vm.SelectedName).Select(GetSelectedViewModel).ToPropertyEx(this, vm => vm.SelectedViewModel);
        ManageSnacksCommand = ReactiveCommand.Create(ManageSnacks);
        ManageSnackMachinesCommand = ReactiveCommand.Create(ManageSnackMachines);
    }

    private readonly Dictionary<string, ReactiveObject> _viewModels = new()
                                                                      {
                                                                          { "Snacks", new SnacksManagementViewModel() },
                                                                          { "SnackMachines", new SnackMachinesManagementViewModel() }
                                                                      };

    private ReactiveObject GetSelectedViewModel(string? name)
    {
        name ??= "Snacks";
        if (!_viewModels.TryGetValue(name, out var viewModel))
        {
            viewModel = _viewModels["Snacks"];
        }
        return viewModel;
    }

    #region Properties

    [ObservableAsProperty]
    public ReactiveObject? SelectedViewModel { get; }

    [Reactive]
    public string? SelectedName { get; set; }

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Unit> ManageSnacksCommand { get; }

    private void ManageSnacks()
    {
        SelectedName = "Snacks";
    }

    public ReactiveCommand<Unit, Unit> ManageSnackMachinesCommand { get; }

    private void ManageSnackMachines()
    {
        SelectedName = "SnackMachines";
    }

    #endregion

}
