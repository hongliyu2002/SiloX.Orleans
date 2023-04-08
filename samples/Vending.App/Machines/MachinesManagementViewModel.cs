using Fluxera.Guards;
using ReactiveUI;

namespace Vending.App.Machines;

public class MachinesManagementViewModel : ReactiveObject
{
    private readonly MainWindowModel _mainModel;

    /// <inheritdoc />
    public MachinesManagementViewModel(MainWindowModel mainModel)
    {
        _mainModel = Guard.Against.Null(mainModel, nameof(mainModel));
    }
}