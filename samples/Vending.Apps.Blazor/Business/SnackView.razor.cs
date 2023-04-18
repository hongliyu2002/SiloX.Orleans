using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using ReactiveUI;
using ReactiveUI.Blazor;

namespace Vending.Apps.Blazor.Business;

public partial class SnackView : ReactiveComponentBase<SnackViewModel>
{
    private IDisposable? _viewModelChangedSubscription;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        Deactivate();
        base.Dispose(disposing);
    }

    [Parameter]
    public SnackViewModel? DisplayViewModel
    {
        get => ViewModel;
        set
        {
            if (ViewModel == value)
            {
                return;
            }
            Deactivate();
            ViewModel = value;
            Activate();
        }
    }

    private void Activate()
    {
        if (ViewModel == null)
        {
            return;
        }
        _viewModelChangedSubscription = this.WhenAnyObservable(v => v.ViewModel!.Changed)
                                            .Throttle(TimeSpan.FromMilliseconds(200))
                                            .Subscribe(_ => InvokeAsync(StateHasChanged));
        ViewModel.Activator.Activate();
    }

    private void Deactivate()
    {
        _viewModelChangedSubscription?.Dispose();
        ViewModel?.Activator.Deactivate();
    }
}