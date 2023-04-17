using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orleans.FluentResults;
using ReactiveUI;
using ReactiveUI.Blazor;
using SiloX.Domain.Abstractions.Extensions;

namespace Vending.Apps.Blazor.Machines;

public partial class MachinesManagementView : ReactiveInjectableComponentBase<MachinesManagementViewModel>
{
    private IDisposable? _confirmRemoveMachineInteractionHandler;
    private IDisposable? _errorsInteractionHandler;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.WhenAnyValue(v => v.ViewModel!.Changed)
            .Throttle(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ => InvokeAsync(StateHasChanged));
        ViewModel?.Activator.Activate();
        _confirmRemoveMachineInteractionHandler = ViewModel?.ConfirmRemoveMachineInteraction.RegisterHandler(ConfirmRemoveMachine);
        _errorsInteractionHandler = ViewModel?.ErrorsInteraction.RegisterHandler(HandleErrors);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        ViewModel?.Activator.Deactivate();
        _confirmRemoveMachineInteractionHandler?.Dispose();
        _errorsInteractionHandler?.Dispose();
        base.Dispose(disposing);
    }

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    private async Task ConfirmRemoveMachine(InteractionContext<string, bool> interaction)
    {
        var result = await DialogService.ShowMessageBox("Confirm", $"Are you sure you want to remove {interaction.Input}?", "Yes", "No");
        interaction.SetOutput(result == true);
    }

    private async Task HandleErrors(InteractionContext<IEnumerable<IError>, ErrorRecovery> interaction)
    {
        var errors = interaction.Input;
        var message = errors.ToMessage();
        var result = await DialogService.ShowMessageBox("Errors occurred when operating", (MarkupString)$">{message}.\n## Retry or cancel?", "Retry", "Abort");
        interaction.SetOutput(result == true ? ErrorRecovery.Retry : ErrorRecovery.Abort);
    }
}