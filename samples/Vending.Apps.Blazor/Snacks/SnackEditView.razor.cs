using Microsoft.AspNetCore.Components;

namespace Vending.Apps.Blazor.Snacks;

public partial class SnackEditView
{
 /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        ViewModel?.Activator.Deactivate();
        base.Dispose(disposing);
    }

    [Parameter]
    public SnackEditViewModel? EditViewModel
    {
        get => ViewModel;
        set
        {
            if (ViewModel == value)
            {
                return;
            }
            ViewModel?.Activator.Deactivate();
            ViewModel = value;
            ViewModel?.Activator.Activate();
        }
    }
}