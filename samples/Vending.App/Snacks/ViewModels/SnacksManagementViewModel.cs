using System.Reactive;
using ReactiveUI;

namespace Vending.App.ViewModels;

public class SnacksManagementViewModel : ReactiveObject
{

    /// <inheritdoc />
    public SnacksManagementViewModel()
    {
        AddSnackCommand = ReactiveCommand.Create(AddSnack);
        RemoveSnackCommand = ReactiveCommand.Create(RemoveSnack);
        MoveNavigationSideCommand = ReactiveCommand.Create(MoveNavigationSide);
    }

    #region Properties

    private NavigationSide _navigationSide;
    public NavigationSide NavigationSide
    {
        get => _navigationSide;
        set => this.RaiseAndSetIfChanged(ref _navigationSide, value);
    }

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Unit> AddSnackCommand { get; }

    private void AddSnack()
    {
    }

    public ReactiveCommand<Unit, Unit> RemoveSnackCommand { get; }

    private void RemoveSnack()
    {
    }

    public ReactiveCommand<Unit, Unit> MoveNavigationSideCommand { get; }

    private void MoveNavigationSide()
    {
        NavigationSide = NavigationSide == NavigationSide.Left ? NavigationSide.Right : NavigationSide.Left;
    }

    #endregion

}
