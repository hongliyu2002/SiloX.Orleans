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
    private readonly Dictionary<string, ReactiveObject> _viewModels;

    public MainWindowModel()
    {
        var appLifetime = Locator.Current.GetService<IHostApplicationLifetime>();
        if (appLifetime != null)
        {
            appLifetime.ApplicationStarted.Register(() => ClusterClient = Locator.Current.GetService<IClusterClient>());
            appLifetime.ApplicationStopped.Register(() => ClusterClient = null);
        }
        _viewModels = new Dictionary<string, ReactiveObject>
                      {
                          { "Snacks", new SnacksManagementViewModel(this) },
                          { "Machines", new MachinesManagementViewModel(this) },
                          { "Purchases", new PurchasesManagementViewModel(this) }
                      };
        CurrentViewModel ??= _viewModels["Snacks"];
        GoSnacksManagementCommand = ReactiveCommand.Create(GoSnacksManagement);
        GoMachinesManagementCommand = ReactiveCommand.Create(GoMachinesManagement);
        GoPurchasesManagementCommand = ReactiveCommand.Create(GoPurchasesManagement);
    }

    #region Properties

    [Reactive]
    public IClusterClient? ClusterClient { get; private set; }

    [Reactive]
    public ReactiveObject? CurrentViewModel { get; private set; }

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