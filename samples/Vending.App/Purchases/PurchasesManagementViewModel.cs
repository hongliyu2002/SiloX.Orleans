using Fluxera.Guards;
using ReactiveUI;

namespace Vending.App.Purchases;

public class PurchasesManagementViewModel : ReactiveObject
{
    private readonly MainWindowModel _mainModel;

    /// <inheritdoc />
    public PurchasesManagementViewModel(MainWindowModel mainModel)
    {
        _mainModel = Guard.Against.Null(mainModel, nameof(mainModel));
    }
}