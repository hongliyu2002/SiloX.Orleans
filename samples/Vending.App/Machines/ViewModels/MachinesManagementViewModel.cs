using Fluxera.Guards;
using ReactiveUI;

namespace Vending.App.ViewModels;

public class MachinesManagementViewModel : ReactiveObject
{
    private readonly MainViewModel _main;

    /// <inheritdoc />
    public MachinesManagementViewModel(MainViewModel main)
    {
        _main = Guard.Against.Null(main, nameof(main));
    }
}