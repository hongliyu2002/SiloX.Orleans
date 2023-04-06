using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace Vending.App.ViewModels;

public class MainViewModel : ReactiveObject
{
    public MainViewModel()
    {
        _selectedViewModel = this.WhenAnyValue(vm => vm.SelectedName)
                                 .Select(GetSelectedViewModel)
                                 .ToProperty(this, vm => vm.SelectedViewModel);
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

    private readonly ObservableAsPropertyHelper<ReactiveObject> _selectedViewModel;
    public ReactiveObject SelectedViewModel => _selectedViewModel.Value;

    private string? _selectedName;
    public string? SelectedName
    {
        get => _selectedName;
        set => this.RaiseAndSetIfChanged(ref _selectedName, value);
    }

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
