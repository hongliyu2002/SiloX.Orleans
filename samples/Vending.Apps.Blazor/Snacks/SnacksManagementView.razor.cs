using System.Reactive.Linq;
using ReactiveUI;

namespace Vending.Apps.Blazor.Snacks;

public partial class SnacksManagementView
{
    /// <inheritdoc />
    protected override void OnInitialized()
    {
        this.WhenAnyValue(v => v.ViewModel!.Changed)
            .Throttle(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ => InvokeAsync(StateHasChanged));
        base.OnInitialized();
    }

    private async Task<object> AddSnackAsync()
    {
        return null;
    }

    private async Task<object> RemoveSnackAsync()
    {
        return null;
    }

    private void MoveNavigationSide()
    {
    }
}