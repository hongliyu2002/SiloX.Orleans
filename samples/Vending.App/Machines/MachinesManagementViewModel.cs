﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using DynamicData;
using DynamicData.Binding;
using Orleans;
using Orleans.FluentResults;
using Orleans.Runtime;
using Orleans.Streams;
using ReactiveUI;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;
using Vending.Projection.Abstractions.Machines;

namespace Vending.App.Machines;

public class MachinesManagementViewModel : ReactiveObject, IActivatableViewModel, IOrleansObject
{
    private readonly SourceCache<MachineViewModel, Guid> _machinesCache;
    private readonly ReadOnlyObservableCollection<MachineViewModel> _machines;
    private StreamSubscriptionHandle<MachineInfoEvent>? _subscription;
    private StreamSequenceToken? _lastSequenceToken;

    /// <inheritdoc />
    public MachinesManagementViewModel()
    {
        // Create the cache for the machines.
        _machinesCache = new SourceCache<MachineViewModel, Guid>(machine => machine.Id);
        _machinesCache.Connect()
                      .Filter(this.WhenAnyValue(vm => vm.PageSize, vm => vm.PageNumber, vm => vm.MoneyAmountStart, vm => vm.MoneyAmountEnd)
                                  .Throttle(TimeSpan.FromMilliseconds(500))
                                  .DistinctUntilChanged()
                                  .Select(query => new Func<MachineViewModel, bool>(machine => (query.Item3 == null || machine.MoneyInside.Amount >= query.Item3)
                                                                                                && (query.Item4 == null || machine.MoneyInside.Amount < query.Item4)
                                                                                                && machine.IsDeleted == false)))
                      .Sort(SortExpressionComparer<MachineViewModel>.Ascending(machine => machine.Id))
                      .Skip(PageSize * (PageNumber - 1))
                      .Take(PageSize)
                      .ObserveOn(RxApp.MainThreadScheduler)
                      .Bind(out _machines)
                      .Subscribe();
        // Recalculate the page number when the page size or page count changes.
        this.WhenAnyValue(vm => vm.PageSize, vm => vm.PageCount)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .DistinctUntilChanged()
            .Subscribe(page =>
                       {
                           var pageNumber = PageNumber <= 0 ? 1 : PageNumber;
                           var oldOffset = _oldPageSize * (pageNumber - 1);
                           var newPageOfOldOffset = (int)Math.Ceiling((double)oldOffset / PageSize);
                           PageNumber = Math.Min(newPageOfOldOffset, page.Item2);
                       });
        // Search machines and update the cache.
        this.WhenAnyValue(vm => vm.MoneyAmountStart, vm => vm.MoneyAmountEnd, vm => vm.ClusterClient)
            .Where(moneyClient => moneyClient.Item3 != null)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .DistinctUntilChanged()
            .Select(moneyClient => (moneyClient.Item1, moneyClient.Item2, moneyClient.Item3!.GetGrain<IMachineRetrieverGrain>("Manager")))
            .SelectMany(moneyGrain => moneyGrain.Item3.ListAsync(new MachineRetrieverListQuery(new Dictionary<string, bool> { { "Id", false } }, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager", new DecimalRange(moneyGrain.Item1, moneyGrain.Item2))))
            .Where(result => result.IsSuccess)
            .Select(result => result.Value)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(machines =>
                       {
                           _machinesCache.Edit(updater => updater.AddOrUpdate(machines.Select(machine => new MachineViewModel(machine))));
                           PageCount = (int)Math.Ceiling((double)machines.Count / PageSize);
                       });
        this.WhenActivated(disposable =>
                           {
                               // When the cluster client changes, subscribe to the machine info stream.
                               this.WhenAnyValue(vm => vm.ClusterClient)
                                   .Where(client => client != null)
                                   .Select(client => client!.GetStreamProvider(Constants.StreamProviderName))
                                   .Select(provider => provider.GetStream<MachineInfoEvent>(StreamId.Create(Constants.MachineInfosBroadcastNamespace, Guid.Empty)))
                                   .SelectMany(stream => stream.SubscribeAsync(HandleEventAsync, HandleErrorAsync, HandleCompletedAsync, _lastSequenceToken))
                                   .ObserveOn(RxApp.MainThreadScheduler)
                                   .Subscribe(HandleSubscriptionAsync)
                                   .DisposeWith(disposable);
                               Disposable.Create(HandleSubscriptionDisposeAsync)
                                         .DisposeWith(disposable);
                           });
        // Create the commands.
        AddMachineCommand = ReactiveCommand.Create(AddMachineAsync, CanAddMachine);
        RemoveMachineCommand = ReactiveCommand.CreateFromTask(RemoveMachineAsync, CanRemoveMachine);
        GoPreviousPageCommand = ReactiveCommand.Create(GoPreviousPage, CanGoPreviousPage);
        GoNextPageCommand = ReactiveCommand.Create(GoNextPage, CanGoNextPage);
    }

    #region Stream Handlers

    private async void HandleSubscriptionAsync(StreamSubscriptionHandle<MachineInfoEvent> subscription)
    {
        if (_subscription != null)
        {
            try
            {
                await _subscription.UnsubscribeAsync();
            }
            catch
            {
                // ignored
            }
        }
        _subscription = subscription;
    }

    private async void HandleSubscriptionDisposeAsync()
    {
        if (_subscription == null)
        {
            return;
        }
        try
        {
            await _subscription.UnsubscribeAsync();
        }
        catch
        {
            // ignored
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
        _machinesCache.Edit(updater => updater.AddOrUpdate(new MachineViewModel(machineEvent.Machine)));
        return Task.CompletedTask;
    }

    private Task ApplyErrorEventAsync(MachineInfoErrorEvent errorEvent)
    {
        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    public ViewModelActivator Activator { get; } = new();

    private IClusterClient? _clusterClient;

    public IClusterClient? ClusterClient
    {
        get => _clusterClient;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _clusterClient, value);
                               });
        }
    }

    private int _pageSize = 10;
    private int _oldPageSize;

    public int PageSize
    {
        get => _pageSize;
        set
        {
            _oldPageSize = _pageSize;
            this.RaiseAndSetIfChanged(ref _pageSize, value);
        }
    }

    private int _pageCount;

    public int PageCount
    {
        get => _pageCount;
        set => this.RaiseAndSetIfChanged(ref _pageCount, value);
    }

    private int _pageNumber = 1;

    public int PageNumber
    {
        get => _pageNumber;
        set => this.RaiseAndSetIfChanged(ref _pageNumber, value);
    }

    private decimal? _moneyAmountStart;

    public decimal? MoneyAmountStart
    {
        get => _moneyAmountStart;
        set => this.RaiseAndSetIfChanged(ref _moneyAmountStart, value);
    }

    private decimal? _moneyAmountEnd;

    public decimal? MoneyAmountEnd
    {
        get => _moneyAmountEnd;
        set => this.RaiseAndSetIfChanged(ref _moneyAmountEnd, value);
    }

    private MachineViewModel? _currentMachine;

    public MachineViewModel? CurrentMachine
    {
        get => _currentMachine;
        set => this.RaiseAndSetIfChanged(ref _currentMachine, value);
    }

    public ReadOnlyObservableCollection<MachineViewModel> Machines => _machines;

    #endregion

    #region Commands

    /// <summary>
    ///     Gets the command that adds a new machine.
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddMachineCommand { get; }

    private IObservable<bool> CanAddMachine =>
        this.WhenAnyValue(vm => vm.ClusterClient)
            .Select(client => client != null);

    /// <summary>
    ///     Gets the command that moves the navigation side.
    /// </summary>
    private void AddMachineAsync()
    {
        // IMachineRepoGrain repoGrain = null!;
        // Result.Ok()
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
        this.WhenAnyValue(vm => vm.CurrentMachine, vm => vm.ClusterClient)
            .Select(machineClient => machineClient is { Item1: not null, Item2: not null });

    /// <summary>
    ///     Gets the interaction that asks the user to confirm the removal of the current machine.
    /// </summary>
    public Interaction<string, bool> ConfirmRemoveMachine { get; } = new();

    /// <summary>
    ///     Removes the current machine.
    /// </summary>
    private async Task RemoveMachineAsync()
    {
        var confirm = await ConfirmRemoveMachine.Handle(CurrentMachine!.Id.ToString());
        if (!confirm)
        {
            return;
        }
        bool retry;
        do
        {
            var result = await Result.Ok()
                                     .MapTry(() => ClusterClient!.GetGrain<IMachineRepoGrain>(string.Empty))
                                     .BindTryAsync(grain => grain.DeleteAsync(new MachineRepoDeleteCommand(CurrentMachine!.Id, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")));
            if (result.IsSuccess)
            {
                return;
            }
            var errorRecovery = await Interactions.Errors.Handle(result.Errors);
            retry = errorRecovery == ErrorRecoveryOption.Retry;
        }
        while (retry);
    }

    /// <summary>
    ///     Gets the command that moves to the previous page.
    /// </summary>
    public ReactiveCommand<Unit, Unit> GoPreviousPageCommand { get; }

    private IObservable<bool> CanGoPreviousPage =>
        this.WhenAnyValue(vm => vm.PageNumber)
            .Select(page => page > 1);

    /// <summary>
    ///     Moves to the previous page.
    /// </summary>
    private void GoPreviousPage()
    {
        PageNumber--;
    }

    /// <summary>
    ///     Gets the command that moves to the next page.
    /// </summary>
    public ReactiveCommand<Unit, Unit> GoNextPageCommand { get; }

    private IObservable<bool> CanGoNextPage =>
        this.WhenAnyValue(vm => vm.PageNumber, vm => vm.PageCount)
            .Select(page => page.Item1 < page.Item2);

    /// <summary>
    ///     Moves to the next page.
    /// </summary>
    private void GoNextPage()
    {
        PageNumber++;
    }

    #endregion

}