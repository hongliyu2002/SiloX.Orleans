using System.Reactive.Linq;
using ReactiveUI;

namespace Vending.Apps.Blazor.Snacks;

public partial class SnacksManagementView
{
    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.WhenAnyValue(v => v.ViewModel!.Changed)
            .Throttle(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ => InvokeAsync(StateHasChanged));
        ViewModel?.Activator.Activate();
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        ViewModel?.Activator.Deactivate();
        base.Dispose(disposing);
    }
}