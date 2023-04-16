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
        // this.WhenActivated(disposable =>
        //                    {
        //                        this.Bind(ViewModel, vm => vm.SearchTerm, v => v.SearchTextBox.Text).DisposeWith(disposable);
        //                        this.BindCommand(ViewModel, vm => vm.AddSnackCommand, v => v.AddSnackButton).DisposeWith(disposable);
        //                        this.BindCommand(ViewModel, vm => vm.RemoveSnackCommand, v => v.RemoveSnackButton).DisposeWith(disposable);
        //                        this.BindCommand(ViewModel, vm => vm.MoveNavigationSideCommand, v => v.MoveNavigationSideButton).DisposeWith(disposable);
        //                    });
        // this.Bind(ViewModel, vm => vm.SearchTerm, v => v.SearchTextBox.Text);
        this.WhenAnyValue(v => v.ViewModel!.Changed)
            .Throttle(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ => InvokeAsync(StateHasChanged));
        base.OnInitialized();
    }
}