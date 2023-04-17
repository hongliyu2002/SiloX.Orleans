using System.Reactive;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orleans.FluentResults;
using ReactiveUI;
using ReactiveUI.Blazor;
using SiloX.Domain.Abstractions.Extensions;

namespace Vending.Apps.Blazor.Machines;

public partial class MachineEditView : ReactiveComponentBase<MachineEditViewModel>
{
    private IDisposable? _confirmRemoveSlotInteractionHandler;
    private IDisposable? _notifySavedMachineInteractionHandler;
    private IDisposable? _errorsInteractionHandler;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.WhenAnyValue(v => v.ViewModel!.Changed)
            .Throttle(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ => InvokeAsync(StateHasChanged));
        ViewModel?.Activator.Activate();
        _confirmRemoveSlotInteractionHandler = ViewModel?.ConfirmRemoveSlotInteraction.RegisterHandler(ConfirmRemoveSlot);
        _notifySavedMachineInteractionHandler = ViewModel?.NotifySavedMachineInteraction.RegisterHandler(NotifySavedMachine);
        _errorsInteractionHandler = ViewModel?.ErrorsInteraction.RegisterHandler(HandleErrors);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        ViewModel?.Activator.Deactivate();
        _confirmRemoveSlotInteractionHandler?.Dispose();
        _notifySavedMachineInteractionHandler?.Dispose();
        _errorsInteractionHandler?.Dispose();
        base.Dispose(disposing);
    }

    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = default!;

    [Parameter]
    public MachineEditViewModel? EditViewModel
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

    private void Close()
    {
        MudDialog.Cancel();
    }

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    private async Task ConfirmRemoveSlot(InteractionContext<string, bool> interaction)
    {
        var result = await DialogService.ShowMessageBox("Confirm", $"Are you sure you want to remove {interaction.Input}?", "Yes", "No");
        interaction.SetOutput(result == true);
    }

    private async Task NotifySavedMachine(InteractionContext<string, Unit> interaction)
    {
        await DialogService.ShowMessageBox("Notification", interaction.Input, "Ok");
        interaction.SetOutput(Unit.Default);
    }

    private async Task HandleErrors(InteractionContext<IEnumerable<IError>, ErrorRecovery> interaction)
    {
        var errors = interaction.Input;
        var message = errors.ToMessage();
        var result = await DialogService.ShowMessageBox("Errors occurred when operating", (MarkupString)$">{message}.\n\n Retry or cancel?", "Retry", "Abort");
        interaction.SetOutput(result == true ? ErrorRecovery.Retry : ErrorRecovery.Abort);
    }
}