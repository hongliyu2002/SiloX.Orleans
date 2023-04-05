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

    private ReactiveObject GetSelectedViewModel(string? name)
    {
        return name switch
               {
                   "Snacks" => new SnacksManagementViewModel(),
                   "SnackMachines" => new SnackMachinesManagementViewModel(),
                   _ => new SnacksManagementViewModel()
               };
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
