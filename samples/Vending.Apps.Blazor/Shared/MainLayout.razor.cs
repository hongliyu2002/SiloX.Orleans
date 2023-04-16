using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orleans.FluentResults;
using ReactiveUI;
using SiloX.Domain.Abstractions.Extensions;

namespace Vending.Apps.Blazor;

public partial class MainLayout
{
    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    private bool _drawerOpen = true;

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    #region Errors Interactions


    private async Task HandleException(InteractionContext<Exception, ErrorRecovery> exceptionInteraction)
    {
        var exception = exceptionInteraction.Input;
        var message = exception.Message;
        var title = $"Exception occurred in {exception.GetType().Name}";
        var result = await DialogService.ShowMessageBox("Errors occurred when operating", (MarkupString)$">{message}.\n## Retry or cancel?", "Retry", "Abort");
        exceptionInteraction.SetOutput(result == true ? ErrorRecovery.Retry : ErrorRecovery.Abort);
    }

    private async Task HandleErrors(InteractionContext<IEnumerable<IError>, ErrorRecovery> errorsInteraction)
    {
        var errors = errorsInteraction.Input;
        var message = errors.ToMessage();
        var result = await DialogService.ShowMessageBox("Errors occurred when operating", (MarkupString)$">{message}.\n## Retry or cancel?", "Retry", "Abort");
        errorsInteraction.SetOutput(result == true ? ErrorRecovery.Retry : ErrorRecovery.Abort);
    }

    #endregion

}