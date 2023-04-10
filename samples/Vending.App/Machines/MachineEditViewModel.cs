// using System;
// using System.Reactive;
// using System.Reactive.Disposables;
// using System.Reactive.Linq;
// using System.Threading.Tasks;
// using System.Windows;
// using Fluxera.Guards;
// using Fluxera.Utilities.Extensions;
// using Orleans;
// using Orleans.FluentResults;
// using Orleans.Runtime;
// using Orleans.Streams;
// using ReactiveUI;
// using Vending.Domain.Abstractions;
// using Vending.Domain.Abstractions.Machines;
//
// namespace Vending.App.Machines;
//
// public class MachineEditViewModel : ReactiveObject, IActivatableViewModel, IOrleansObject
// {
//     private StreamSubscriptionHandle<MachineEvent>? _subscription;
//     private StreamSequenceToken? _lastSequenceToken;
//
//     public MachineEditViewModel(Machine machine, IClusterClient clusterClient)
//     {
//         Guard.Against.Null(machine, nameof(machine));
//         ClusterClient = Guard.Against.Null(clusterClient, nameof(clusterClient));
//         LoadMachine(machine);
//         this.WhenActivated(disposable =>
//                            {
//                                // When the cluster client changes, subscribe to the machine info stream.
//                                this.WhenAnyValue(vm => vm.Id, vm => vm.ClusterClient)
//                                    .Where(idClient => idClient.Item1 != Guid.Empty && idClient.Item2 != null)
//                                    .Select(idClient => (idClient.Item1, idClient.Item2!.GetStreamProvider(Constants.StreamProviderName)))
//                                    .Select(idProvider => idProvider.Item2.GetStream<MachineEvent>(StreamId.Create(Constants.MachinesNamespace, idProvider.Item1)))
//                                    .SelectMany(stream => stream.SubscribeAsync(HandleEventAsync, HandleErrorAsync, HandleCompletedAsync, _lastSequenceToken))
//                                    .ObserveOn(RxApp.MainThreadScheduler)
//                                    .Subscribe(HandleSubscriptionAsync)
//                                    .DisposeWith(disposable);
//                                Disposable.Create(HandleSubscriptionDisposeAsync)
//                                          .DisposeWith(disposable);
//                            });
//         // Create the commands.
//         SaveMachineCommand = ReactiveCommand.CreateFromTask(SaveMachineAsync, CanSaveMachine);
//     }
//
//     #region Stream Handlers
//
//     private async void HandleSubscriptionAsync(StreamSubscriptionHandle<MachineEvent> subscription)
//     {
//         if (_subscription != null)
//         {
//             try
//             {
//                 await _subscription.UnsubscribeAsync();
//             }
//             catch
//             {
//                 // ignored
//             }
//         }
//         _subscription = subscription;
//     }
//
//     private async void HandleSubscriptionDisposeAsync()
//     {
//         if (_subscription == null)
//         {
//             return;
//         }
//         try
//         {
//             await _subscription.UnsubscribeAsync();
//         }
//         catch
//         {
//             // ignored
//         }
//     }
//
//     private Task HandleEventAsync(MachineEvent domainEvent, StreamSequenceToken sequenceToken)
//     {
//         _lastSequenceToken = sequenceToken;
//         switch (domainEvent)
//         {
//             case MachineInitializedEvent machineEvent:
//                 return ApplyEventAsync(machineEvent);
//             case MachineDeletedEvent machineEvent:
//                 return ApplyEventAsync(machineEvent);
//             case MachineUpdatedEvent machineEvent:
//                 return ApplyEventAsync(machineEvent);
//             case MachineErrorEvent machineEvent:
//                 return ApplyErrorEventAsync(machineEvent);
//             default:
//                 return Task.CompletedTask;
//         }
//     }
//
//     private Task HandleErrorAsync(Exception exception)
//     {
//         return Task.CompletedTask;
//     }
//
//     private Task HandleCompletedAsync()
//     {
//         return Task.CompletedTask;
//     }
//
//     private Task ApplyEventAsync(MachineInitializedEvent machineEvent)
//     {
//         if (machineEvent.MachineId == Id)
//         {
//             Name = machineEvent.Name;
//             PictureUrl = machineEvent.PictureUrl;
//         }
//         return Task.CompletedTask;
//     }
//
//     private Task ApplyEventAsync(MachineDeletedEvent machineEvent)
//     {
//         if (machineEvent.MachineId == Id)
//         {
//             IsDeleted = true;
//         }
//         return Task.CompletedTask;
//     }
//
//     private Task ApplyEventAsync(MachineUpdatedEvent machineEvent)
//     {
//         if (machineEvent.MachineId == Id)
//         {
//             Name = machineEvent.Name;
//             PictureUrl = machineEvent.PictureUrl;
//         }
//         return Task.CompletedTask;
//     }
//
//     private Task ApplyErrorEventAsync(MachineErrorEvent errorEvent)
//     {
//         return Task.CompletedTask;
//     }
//
//     #endregion
//
//     #region Properties
//
//     /// <inheritdoc />
//     public ViewModelActivator Activator { get; } = new();
//
//     private IClusterClient? _clusterClient;
//
//     public IClusterClient? ClusterClient
//     {
//         get => _clusterClient;
//         set
//         {
//             var dispatcher = Application.Current.Dispatcher;
//             dispatcher?.Invoke(() =>
//                                {
//                                    this.RaiseAndSetIfChanged(ref _clusterClient, value);
//                                });
//         }
//     }
//
//     private Guid _id;
//
//     public Guid Id
//     {
//         get => _id;
//         set
//         {
//             var dispatcher = Application.Current.Dispatcher;
//             dispatcher?.Invoke(() =>
//                                {
//                                    this.RaiseAndSetIfChanged(ref _id, value);
//                                });
//         }
//     }
//
//     private string _name = string.Empty;
//
//     public string Name
//     {
//         get => _name;
//         set
//         {
//             var dispatcher = Application.Current.Dispatcher;
//             dispatcher?.Invoke(() =>
//                                {
//                                    this.RaiseAndSetIfChanged(ref _name, value);
//                                });
//         }
//     }
//
//     private string? _pictureUrl;
//
//     public string? PictureUrl
//     {
//         get => _pictureUrl;
//         set
//         {
//             var dispatcher = Application.Current.Dispatcher;
//             dispatcher?.Invoke(() =>
//                                {
//                                    this.RaiseAndSetIfChanged(ref _pictureUrl, value);
//                                });
//         }
//     }
//
//     private bool _isDeleted;
//
//     public bool IsDeleted
//     {
//         get => _isDeleted;
//         set
//         {
//             var dispatcher = Application.Current.Dispatcher;
//             dispatcher?.Invoke(() =>
//                                {
//                                    this.RaiseAndSetIfChanged(ref _isDeleted, value);
//                                });
//         }
//     }
//
//     #endregion
//
//     #region Commands
//
//     /// <summary>
//     ///     Gets the command to save the machine.
//     /// </summary>
//     public ReactiveCommand<Unit, Unit> SaveMachineCommand { get; }
//
//     private IObservable<bool> CanSaveMachine =>
//         this.WhenAnyValue(vm => vm.Name, vm => vm.IsDeleted, vm => vm.ClusterClient)
//             .Select(nameClient => nameClient.Item1.IsNotNullOrEmpty() && nameClient is { Item2: false, Item3: not null });
//
//     private async Task SaveMachineAsync()
//     {
//         bool retry;
//         do
//         {
//             IMachineRepoGrain repoGrain = null!;
//             var result = await Result.Ok()
//                                      .MapTry(() => repoGrain = ClusterClient!.GetGrain<IMachineRepoGrain>("Manager"))
//                                      .BindTryIfAsync(Id == Guid.Empty, () => repoGrain.CreateAsync(new MachineRepoCreateCommand(Name, PictureUrl, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
//                                      .BindTryIfAsync<Machine>(Id != Guid.Empty, () => repoGrain.UpdateAsync(new MachineRepoUpdateCommand(Id, Name, PictureUrl, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
//                                      .TapTryAsync(LoadMachine);
//             if (result.IsSuccess)
//             {
//                 return;
//             }
//             var errorRecovery = await Interactions.Errors.Handle(result.Errors);
//             retry = errorRecovery == ErrorRecoveryOption.Retry;
//         }
//         while (retry);
//     }
//
//     #endregion
//
//     #region Load Machine
//
//     private void LoadMachine(Machine machine)
//     {
//         Id = machine.Id;
//         Name = machine.Name;
//         PictureUrl = machine.PictureUrl;
//         IsDeleted = machine.IsDeleted;
//     }
//
//     #endregion
//
// }