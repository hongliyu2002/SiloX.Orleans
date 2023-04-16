using System.Reactive.Disposables;
using System.Reactive.Linq;
using MudBlazor;
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
}