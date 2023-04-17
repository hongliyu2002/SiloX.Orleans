using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orleans.FluentResults;
using ReactiveUI;
using ReactiveUI.Blazor;
using SiloX.Domain.Abstractions.Extensions;

namespace Vending.Apps.Blazor.Snacks;

public partial class SnacksManagementView : ReactiveInjectableComponentBase<SnacksManagementViewModel>
{
    private IDisposable? _viewModelChangedSubscription;
    private IDisposable? _confirmRemoveSnackInteractionHandler;
    private IDisposable? _errorsInteractionHandler;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Activate();
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        Deactivate();
        base.Dispose(disposing);
    }

    private void Activate()
    {
        if (ViewModel == null)
        {
            return;
        }
        _viewModelChangedSubscription = this.WhenAnyValue(v => v.ViewModel!.Changed)
                                            .Throttle(TimeSpan.FromMilliseconds(200))
                                            .Subscribe(_ => InvokeAsync(StateHasChanged));
        _confirmRemoveSnackInteractionHandler = ViewModel.ConfirmRemoveSnackInteraction.RegisterHandler(ConfirmRemoveSnack);
        _errorsInteractionHandler = ViewModel.ErrorsInteraction.RegisterHandler(HandleErrors);
        ViewModel.Activator.Activate();
    }

    private void Deactivate()
    {
        _viewModelChangedSubscription?.Dispose();
        _confirmRemoveSnackInteractionHandler?.Dispose();
        _errorsInteractionHandler?.Dispose();
        ViewModel?.Activator.Deactivate();
    }

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    private async Task ConfirmRemoveSnack(InteractionContext<string, bool> interaction)
    {
        var result = await DialogService.ShowMessageBox("Confirm", $"Are you sure you want to remove {interaction.Input}?", "Yes", "No");
        interaction.SetOutput(result == true);
    }

    private async Task HandleErrors(InteractionContext<IEnumerable<IError>, ErrorRecovery> interaction)
    {
        var errors = interaction.Input;
        var message = errors.ToMessage();
        var result = await DialogService.ShowMessageBox("Errors occurred when operating", (MarkupString)$">{message}.\n\n Retry or cancel?", "Retry", "Abort");
        interaction.SetOutput(result == true ? ErrorRecovery.Retry : ErrorRecovery.Abort);
    }
}