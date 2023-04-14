using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Windows;
using Fluxera.Extensions.Hosting;
using Orleans.FluentResults;
using ReactiveUI;
using SiloX.Domain.Abstractions.Extensions;

namespace Vending.App.Wpf;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : IMainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new MainWindowModel();
        this.WhenActivated(disposable =>
                           {
                               this.OneWayBind(ViewModel, vm => vm.CurrentViewModel, v => v.MainViewHost.ViewModel).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.GoSnacksManagementCommand, v => v.SnacksManagementMenuItem).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.GoMachinesManagementCommand, v => v.MachinesManagementMenuItem).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.GoPurchasesManagementCommand, v => v.PurchasesManagementMenuItem).DisposeWith(disposable);
                           });
        Interactions.Exception.RegisterHandler(HandleException);
        Interactions.Errors.RegisterHandler(HandleErrors);
    }

    private static void HandleException(InteractionContext<Exception, ErrorRecoveryOption> exceptionInteraction)
    {
        var exception = exceptionInteraction.Input;
        var message = exception.Message;
        var title = $"Exception occurred in {exception.GetType().Name}";
        var result = MessageBox.Show($"{message}.\nRetry or cancel?", title, MessageBoxButton.OKCancel, MessageBoxImage.Error);
        exceptionInteraction.SetOutput(result == MessageBoxResult.OK ? ErrorRecoveryOption.Retry : ErrorRecoveryOption.Abort);
    }
    
    private static void HandleErrors(InteractionContext<IEnumerable<IError>, ErrorRecoveryOption> errorsInteraction)
    {
        var errors = errorsInteraction.Input;
        var message = errors.ToMessage();
        var result = MessageBox.Show($"{message}.\nRetry or cancel?", "Errors occurred when operating", MessageBoxButton.OKCancel, MessageBoxImage.Error);
        errorsInteraction.SetOutput(result == MessageBoxResult.OK ? ErrorRecoveryOption.Retry : ErrorRecoveryOption.Abort);
    }
}