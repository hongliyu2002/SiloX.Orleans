using System.Collections.Generic;
using System.Reactive;
using System.Windows;
using Microsoft.Extensions.Hosting;
using Orleans;
using ReactiveUI;
using Splat;
using Vending.App.Machines;
using Vending.App.Purchases;
using Vending.App.Snacks;

namespace Vending.App;

public class MainWindowModel : ReactiveObject
{
    private readonly Dictionary<string, IOrleansObject> _viewModels;

    public MainWindowModel()
    {
        _viewModels = new Dictionary<string, IOrleansObject>
                      {
                          { "Snacks", new SnacksManagementViewModel() },
                          { "Machines", new MachinesManagementViewModel() },
                          { "Purchases", new PurchasesManagementViewModel() }
                      };
        CurrentViewModel ??= _viewModels["Snacks"];
        // Create commands
        GoSnacksManagementCommand = ReactiveCommand.Create(GoSnacksManagement);
        GoMachinesManagementCommand = ReactiveCommand.Create(GoMachinesManagement);
        GoPurchasesManagementCommand = ReactiveCommand.Create(GoPurchasesManagement);
        // Register to application started event
        var appLifetime = Locator.Current.GetService<IHostApplicationLifetime>();
        if (appLifetime != null)
        {
            var clusterClient = Locator.Current.GetService<IClusterClient>();
            appLifetime.ApplicationStarted.Register(() =>
                                                    {
                                                        var dispatcher = Application.Current.Dispatcher;
                                                        dispatcher?.Invoke(() =>
                                                                           {
                                                                               _viewModels["Snacks"].ClusterClient = clusterClient;
                                                                               _viewModels["Machines"].ClusterClient = clusterClient;
                                                                               _viewModels["Purchases"].ClusterClient = clusterClient;
                                                                           });
                                                    });
        }
    }

    #region Properties

    private IOrleansObject? _currentViewModel;
    public IOrleansObject? CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Unit> GoSnacksManagementCommand { get; }

    private void GoSnacksManagement()
    {
        CurrentViewModel = _viewModels["Snacks"];
    }

    public ReactiveCommand<Unit, Unit> GoMachinesManagementCommand { get; }

    private void GoMachinesManagement()
    {
        CurrentViewModel = _viewModels["Machines"];
    }

    public ReactiveCommand<Unit, Unit> GoPurchasesManagementCommand { get; }

    private void GoPurchasesManagement()
    {
        CurrentViewModel = _viewModels["Purchases"];
    }

    #endregion

}