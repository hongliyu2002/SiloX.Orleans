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

    public MainWindowModel()
    {
        var appLifetime = Locator.Current.GetService<IHostApplicationLifetime>();
        if (appLifetime != null)
        {
            appLifetime.ApplicationStarted.Register(() => ClusterClient = Locator.Current.GetService<IClusterClient>());
            appLifetime.ApplicationStopped.Register(() => ClusterClient = null);
        }
        var viewModels = new Dictionary<string, ReactiveObject>
                         {
                             { "Snacks", new SnacksManagementViewModel(this) },
                             { "Machines", new MachinesManagementViewModel(this) },
                             { "Purchases", new PurchasesManagementViewModel(this) }
                         };
        CurrentViewModel ??= viewModels["Snacks"];
        GoSnacksManagementCommand = ReactiveCommand.Create(() => CurrentViewModel = viewModels["Snacks"]);
        GoMachinesManagementCommand = ReactiveCommand.Create(() => CurrentViewModel = viewModels["Machines"]);
        GoPurchasesManagementCommand = ReactiveCommand.Create(() => CurrentViewModel = viewModels["Purchases"]);
    }

    #region Properties

    [Reactive]
    public IClusterClient? ClusterClient { get; private set; }

    [Reactive]
    public ReactiveObject? CurrentViewModel { get; private set; }

    #endregion

    #region Commands

    public ReactiveCommand<Unit, ReactiveObject> GoSnacksManagementCommand { get; }

    public ReactiveCommand<Unit, ReactiveObject> GoMachinesManagementCommand { get; }

    public ReactiveCommand<Unit, ReactiveObject> GoPurchasesManagementCommand { get; }

    #endregion

}