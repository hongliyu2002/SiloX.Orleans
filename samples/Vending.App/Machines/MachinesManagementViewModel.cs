using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using Orleans;
using Orleans.FluentResults;
using Orleans.Runtime;
using Orleans.Streams;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;
using Vending.Projection.Abstractions.Machines;

namespace Vending.App.Machines;

public class MachinesManagementViewModel : ReactiveObject, IActivatableViewModel, IHasClusterClient
{
    private readonly SourceCache<MachineItemViewModel, Guid> _machineItemsCache;
    private readonly ReadOnlyObservableCollection<MachineItemViewModel>? _machineItems;
    private StreamSubscriptionHandle<MachineInfoEvent>? _subscription;
    private StreamSequenceToken? _lastSequenceToken;

    /// <inheritdoc />
    public MachinesManagementViewModel()
    {
        // Create the cache for the machine items.
        _machineItemsCache = new SourceCache<MachineItemViewModel, Guid>(machine => machine.Id);
        // Connect to the cache and bind to the machine items.
        var queryConditionObs = this.WhenAnyValue(vm => vm.PageSize, vm => vm.CurrentPage, vm => vm.MoneyAmountStart, vm => vm.MoneyAmountEnd)
                                    .Throttle(TimeSpan.FromMilliseconds(500))
                                    .DistinctUntilChanged()
                                    .Select(query => new Func<MachineItemViewModel, bool>(item => (query.Item3 == null || item.MoneyInside.Amount >= query.Item3) && (query.Item4 == null || item.MoneyInside.Amount < query.Item4)));
        _machineItemsCache.Connect()
                          .Filter(queryConditionObs)
                          .Sort(SortExpressionComparer<MachineItemViewModel>.Ascending(item => item.Id))
                          .Skip(PageSize * (CurrentPage - 1))
                          .Take(PageSize)
                          .ObserveOn(RxApp.MainThreadScheduler)
                          .Bind(out _machineItems)
                          .Subscribe();
        // Search machine items and update the cache.
        this.WhenAnyValue(vm => vm.MoneyAmountStart, vm => vm.MoneyAmountEnd, vm => vm.ClusterClient)
            .Where(moneyClient => moneyClient.Item3 != null)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .DistinctUntilChanged()
            .Select(moneyClient => (moneyClient.Item1, moneyClient.Item2, moneyClient.Item3!.GetGrain<IMachineRetrieverGrain>("Manager")))
            .SelectMany(moneyGrain => moneyGrain.Item3.ListAsync(new MachineRetrieverListQuery(new Dictionary<string, bool> { { "Id", false } }, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager", new DecimalRange(moneyGrain.Item1, moneyGrain.Item2))))
            .Where(result => result.IsSuccess)
            .Select(result => result.Value)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(machines => _machineItemsCache.Edit(updater => updater.AddOrUpdate(machines.Select(machine => new MachineItemViewModel(machine)))));
        this.WhenActivated(disposable =>
                           {
                               // When the cluster client changes, subscribe to the machine info stream.
                               this.WhenAnyValue(vm => vm.ClusterClient)
                                   .Where(client => client != null)
                                   .Select(client => client!.GetStreamProvider(Constants.StreamProviderName))
                                   .Select(provider => provider.GetStream<MachineInfoEvent>(StreamId.Create(Constants.MachineInfosBroadcastNamespace, Guid.Empty)))
                                   .SelectMany(stream => stream.SubscribeAsync(HandleEventAsync, HandleErrorAsync, HandleCompletedAsync, _lastSequenceToken))
                                   .ObserveOn(RxApp.MainThreadScheduler)
                                   .Subscribe(subscription => _subscription = subscription)
                                   .DisposeWith(disposable);
                               Disposable.Create(HandleSubscriptionDisposeAsync)
                                         .DisposeWith(disposable);
                           });
        // Create the commands.
        AddMachineCommand = ReactiveCommand.Create(AddMachineAsync);
        RemoveMachineCommand = ReactiveCommand.CreateFromTask(RemoveMachineAsync, CanRemoveMachine);
        GoPreviousPageCommand = ReactiveCommand.Create(GoPreviousPage, CanGoPreviousPage);
        GoNextPageCommand = ReactiveCommand.Create(GoNextPage);
    }

    #region Stream Handlers

    private async void HandleSubscriptionDisposeAsync()
    {
        if (_subscription != null)
        {
            await _subscription.UnsubscribeAsync();
        }
    }

    private Task HandleEventAsync(MachineInfoEvent projectionEvent, StreamSequenceToken sequenceToken)
    {
        _lastSequenceToken = sequenceToken;
        switch (projectionEvent)
        {
            case MachineInfoSavedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineInfoErrorEvent machineEvent:
                return ApplyErrorEventAsync(machineEvent);
            default:
                return Task.CompletedTask;
        }
    }

    private Task HandleErrorAsync(Exception exception)
    {
        return Task.CompletedTask;
    }

    private Task HandleCompletedAsync()
    {
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(MachineInfoSavedEvent machineEvent)
    {
        _machineItemsCache.Edit(updater => updater.AddOrUpdate(new MachineItemViewModel(machineEvent.Machine)));
        return Task.CompletedTask;
    }

    private Task ApplyErrorEventAsync(MachineInfoErrorEvent machineEvent)
    {
        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    public ViewModelActivator Activator { get; } = new();

    [Reactive]
    public IClusterClient? ClusterClient { get; set; }

    [Reactive]
    public int PageSize { get; set; } = 10;

    [Reactive]
    public int CurrentPage { get; set; } = 1;

    [Reactive]
    public decimal? MoneyAmountStart { get; set; } = 0;

    [Reactive]
    public decimal? MoneyAmountEnd { get; set; } = 10000;

    public ReadOnlyObservableCollection<MachineItemViewModel>? MachineItems { get; }

    [Reactive]
    public MachineItemViewModel? CurrentMachineItem { get; set; }

    #endregion

    #region Commands

    /// <summary>
    ///     Gets the command that adds a new machine.
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddMachineCommand { get; }

    /// <summary>
    ///     Gets the command that moves the navigation side.
    /// </summary>
    private void AddMachineAsync()
    {
        // IMachineRepoGrain repoGrain = null!;
        // Result.Ok()
        //       .Ensure(ClusterClient != null, "Cluster client is not available.")
        //       .MapTry(() => repoGrain = ClusterClient!.GetGrain<IMachineRepoGrain>(string.Empty))
        //       .Map(() => CurrentMachineEdit = new MachineEditViewModel(new Machine(), repoGrain));
    }

    /// <summary>
    ///     Gets the command that removes the current machine.
    /// </summary>
    public ReactiveCommand<Unit, Unit> RemoveMachineCommand { get; }

    /// <summary>
    ///     Gets the observable that indicates whether the remove machine command can be executed.
    /// </summary>
    private IObservable<bool> CanRemoveMachine =>
        this.WhenAnyValue(vm => vm.CurrentMachineItem, vm => vm.ClusterClient)
            .Select(itemClient => itemClient is { Item1: not null, Item2: not null });

    /// <summary>
    ///     Removes the current machine.
    /// </summary>
    private async Task RemoveMachineAsync()
    {
        IMachineRepoGrain repoGrain = null!;
        await Result.Ok()
                    .Ensure(ClusterClient != null, "Cluster client is not available.")
                    .MapTry(() => repoGrain = ClusterClient!.GetGrain<IMachineRepoGrain>(string.Empty))
                    .Ensure(CurrentMachineItem != null, "Machine item should be selected.")
                    .BindTryAsync(() => repoGrain.DeleteAsync(new MachineRepoDeleteCommand(CurrentMachineItem!.Id, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")));
    }

    /// <summary>
    ///     Gets the command that moves to the previous page.
    /// </summary>
    public ReactiveCommand<Unit, Unit> GoPreviousPageCommand { get; }

    private IObservable<bool> CanGoPreviousPage =>
        this.WhenAnyValue(vm => vm.CurrentPage, vm => vm.ClusterClient)
            .Select(pageClient => pageClient is { Item1: > 1, Item2: not null });

    /// <summary>
    ///     Moves to the previous page.
    /// </summary>
    private void GoPreviousPage()
    {
    }

    /// <summary>
    ///     Gets the command that moves to the next page.
    /// </summary>
    public ReactiveCommand<Unit, Unit> GoNextPageCommand { get; }

    /// <summary>
    ///     Moves to the next page.
    /// </summary>
    private void GoNextPage()
    {
    }

    #endregion

}