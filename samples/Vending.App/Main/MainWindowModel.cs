using System.Collections.Generic;
using System.Reactive;
using Microsoft.Extensions.Hosting;
using Orleans;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Vending.App.Machines;
using Vending.App.Purchases;
using Vending.App.Snacks;

namespace Vending.App;

public class MainWindowModel : ReactiveObject
{
    private readonly Dictionary<string, IHasClusterClient> _viewModels;

    public MainWindowModel()
    {
        _viewModels = new Dictionary<string, IHasClusterClient>
                      {
                          { "Snacks", new SnacksManagementViewModel() },
                          { "Machines", new MachinesManagementViewModel() },
                          { "Purchases", new PurchasesManagementViewModel() }
                      };
        CurrentViewModel ??= _viewModels["Snacks"];
        GoSnacksManagementCommand = ReactiveCommand.Create(GoSnacksManagement);
        GoMachinesManagementCommand = ReactiveCommand.Create(GoMachinesManagement);
        GoPurchasesManagementCommand = ReactiveCommand.Create(GoPurchasesManagement);
        var appLifetime = Locator.Current.GetService<IHostApplicationLifetime>();
        if (appLifetime != null)
        {
            var clusterClient = Locator.Current.GetService<IClusterClient>();
            appLifetime.ApplicationStarted.Register(() =>
                                                    {
                                                        _viewModels["Snacks"].ClusterClient = clusterClient;
                                                        _viewModels["Machines"].ClusterClient = clusterClient;
                                                        _viewModels["Purchases"].ClusterClient = clusterClient;
                                                    });
            appLifetime.ApplicationStopped.Register(() =>
                                                    {
                                                        _viewModels["Snacks"].ClusterClient = null;
                                                        _viewModels["Machines"].ClusterClient = null;
                                                        _viewModels["Purchases"].ClusterClient = null;
                                                    });
        }
    }

    #region Properties

    [Reactive]
    public IHasClusterClient? CurrentViewModel { get; set; }

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