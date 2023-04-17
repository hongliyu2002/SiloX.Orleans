﻿using System.Reactive;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orleans.FluentResults;
using ReactiveUI;
using ReactiveUI.Blazor;
using SiloX.Domain.Abstractions.Extensions;

namespace Vending.Apps.Blazor.Snacks;

public partial class SnackEditView : ReactiveComponentBase<SnackEditViewModel>
{
    private IDisposable? _notifySavedSnackInteractionHandler;
    private IDisposable? _errorsInteractionHandler;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _notifySavedSnackInteractionHandler = ViewModel?.NotifySavedSnackInteraction.RegisterHandler(NotifySavedSnack);
        _errorsInteractionHandler = ViewModel?.ErrorsInteraction.RegisterHandler(HandleErrors);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        ViewModel?.Activator.Deactivate();
        _notifySavedSnackInteractionHandler?.Dispose();
        _errorsInteractionHandler?.Dispose();
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

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    private async Task NotifySavedSnack(InteractionContext<string, Unit> interaction)
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