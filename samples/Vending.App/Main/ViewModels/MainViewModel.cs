using System.Collections.Generic;
using System.Reactive;
using Microsoft.Extensions.Hosting;
using Orleans;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Vending.App.ViewModels;

public class MainViewModel : ReactiveObject
{

    public MainViewModel()
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
                             { "Machines", new MachinesManagementViewModel(this) }
                         };
        CurrentViewModel ??= viewModels["Snacks"];
        GoSnacksManagementCommand = ReactiveCommand.Create(() => CurrentViewModel = viewModels["Snacks"]);
        GoMachinesManagementCommand = ReactiveCommand.Create(() => CurrentViewModel = viewModels["Machines"]);
    }

    #region Properties

    [Reactive]
    public IClusterClient? ClusterClient { get; set; }

    [Reactive]
    public ReactiveObject? CurrentViewModel { get; set; }

    #endregion

    #region Commands

    public ReactiveCommand<Unit, ReactiveObject> GoSnacksManagementCommand { get; }

    public ReactiveCommand<Unit, ReactiveObject> GoMachinesManagementCommand { get; }

    #endregion

}