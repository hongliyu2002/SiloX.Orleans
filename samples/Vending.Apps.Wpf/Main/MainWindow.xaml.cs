﻿using System.Reactive.Disposables;
using System.Windows;
using Fluxera.Extensions.Hosting;
using Orleans.FluentResults;
using ReactiveUI;
using SiloX.Domain.Abstractions.Extensions;

namespace Vending.Apps.Wpf;

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
    }
}