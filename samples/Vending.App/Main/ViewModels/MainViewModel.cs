using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace Vending.App.ViewModels;

public class MainViewModel : ReactiveObject
{
    public MainViewModel()
    {
        _selectedViewModel = this.WhenAnyValue(vm => vm.SelectedManagementName)
                                 .Select(GetSelectedViewModel)
                                 .ToProperty(this, vm => vm.SelectedViewModel);
        ManageSnacksCommand = ReactiveCommand.Create(ManageSnacks);
        ManageSnackMachinesCommand = ReactiveCommand.Create(ManageSnackMachines);
    }

    private readonly ObservableAsPropertyHelper<ReactiveObject> _selectedViewModel;
    public ReactiveObject SelectedViewModel => _selectedViewModel.Value;

    private string? _selectedManagementName;
    public string? SelectedManagementName
    {
        get => _selectedManagementName;
        set => this.RaiseAndSetIfChanged(ref _selectedManagementName, value);
    }

    private ReactiveObject GetSelectedViewModel(string? name)
    {
        return name switch
               {
                   "Snacks" => new SnacksManagementViewModel(),
                   "SnackMachines" => new SnackMachinesManagementViewModel(),
                   _ => new SnacksManagementViewModel()
               };
    }

    public ReactiveCommand<Unit, Unit> ManageSnacksCommand { get; }

    private void ManageSnacks()
    {
        SelectedManagementName = "Snacks";
    }

    public ReactiveCommand<Unit, Unit> ManageSnackMachinesCommand { get; }

    private void ManageSnackMachines()
    {
        SelectedManagementName = "SnackMachines";
    }
}
