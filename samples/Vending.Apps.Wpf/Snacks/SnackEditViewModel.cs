using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using Orleans;
using Orleans.FluentResults;
using Orleans.Runtime;
using Orleans.Streams;
using ReactiveUI;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Snacks;

namespace Vending.App.Wpf.Snacks;

public class SnackEditViewModel : ReactiveObject, IActivatableViewModel, IOrleansObject
{
    private StreamSubscriptionHandle<SnackEvent>? _subscription;
    private StreamSequenceToken? _lastSequenceToken;

    public SnackEditViewModel(Snack snack, IClusterClient clusterClient)
    {
        Guard.Against.Null(snack, nameof(snack));
        ClusterClient = Guard.Against.Null(clusterClient, nameof(clusterClient));
        this.WhenActivated(disposable =>
                           {
                               // When the cluster client changes, subscribe to the snack info stream.
                               this.WhenAnyValue(vm => vm.Id, vm => vm.ClusterClient)
                                   .Where(tuple => tuple.Item1 != Guid.Empty && tuple.Item2 != null)
                                   .Select(tuple => (tuple.Item1, tuple.Item2!.GetStreamProvider(Constants.StreamProviderName)))
                                   .Select(tuple => tuple.Item2.GetStream<SnackEvent>(StreamId.Create(Constants.SnacksNamespace, tuple.Item1)))
                                   .SelectMany(stream => stream.SubscribeAsync(HandleEventAsync, HandleErrorAsync, HandleCompletedAsync, _lastSequenceToken))
                                   .Subscribe(HandleSubscriptionAsync)
                                   .DisposeWith(disposable);
                               Disposable.Create(HandleSubscriptionDisposeAsync)
                                         .DisposeWith(disposable);
                           });
        // Create the commands.
        SaveSnackCommand = ReactiveCommand.CreateFromTask(SaveSnackAsync, CanSaveSnack);
        // Load the snack.
        LoadSnack(snack);
    }

    #region Properties

    /// <inheritdoc />
    public ViewModelActivator Activator { get; } = new();

    private IClusterClient? _clusterClient;
    public IClusterClient? ClusterClient
    {
        get => _clusterClient;
        set => this.RaiseAndSetIfChanged(ref _clusterClient, value);
    }

    private Guid _id;
    public Guid Id
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    private string? _pictureUrl;
    public string? PictureUrl
    {
        get => _pictureUrl;
        set => this.RaiseAndSetIfChanged(ref _pictureUrl, value);
    }

    private bool _isDeleted;
    public bool IsDeleted
    {
        get => _isDeleted;
        set => this.RaiseAndSetIfChanged(ref _isDeleted, value);
    }

    #endregion

    #region Commands

    /// <summary>
    ///     Gets the command to save the snack.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SaveSnackCommand { get; }

    private IObservable<bool> CanSaveSnack =>
        this.WhenAnyValue(vm => vm.Name, vm => vm.IsDeleted, vm => vm.ClusterClient)
            .Select(tuple => tuple.Item1.IsNotNullOrEmpty() && tuple is { Item2: false, Item3: not null });

    private async Task SaveSnackAsync()
    {
        bool retry;
        do
        {
            ISnackRepoGrain grain = null!;
            var result = await Result.Ok()
                                     .Ensure(Name.IsNotNullOrEmpty(), "Name is required.")
                                     .Ensure(ClusterClient != null, "No cluster client available.")
                                     .MapTry(() => grain = ClusterClient!.GetGrain<ISnackRepoGrain>("Manager"))
                                     .BindTryIfAsync(Id == Guid.Empty, () => grain.CreateAsync(new SnackRepoCreateCommand(Name, PictureUrl, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
                                     .BindTryIfAsync<Snack>(Id != Guid.Empty, () => grain.UpdateAsync(new SnackRepoUpdateCommand(Id, Name, PictureUrl, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
                                     .TapTryAsync(LoadSnack);
            if (result.IsSuccess)
            {
                return;
            }
            var errorRecovery = await Interactions.Errors.Handle(result.Errors);
            retry = errorRecovery == ErrorRecoveryOption.Retry;
        }
        while (retry);
    }

    #endregion

    #region Load Snack

    private void LoadSnack(Snack snack)
    {
        Id = snack.Id;
        Name = snack.Name;
        PictureUrl = snack.PictureUrl;
        IsDeleted = snack.IsDeleted;
    }

    #endregion

    #region Stream Handlers

    private async void HandleSubscriptionAsync(StreamSubscriptionHandle<SnackEvent> subscription)
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

    private Task HandleEventAsync(SnackEvent domainEvent, StreamSequenceToken sequenceToken)
    {
        _lastSequenceToken = sequenceToken;
        return domainEvent switch
               {
                   SnackInitializedEvent snackEvent => ApplyEventAsync(snackEvent),
                   SnackDeletedEvent snackEvent => ApplyEventAsync(snackEvent),
                   SnackUpdatedEvent snackEvent => ApplyEventAsync(snackEvent),
                   SnackErrorEvent snackEvent => ApplyErrorEventAsync(snackEvent),
                   _ => Task.CompletedTask
               };
    }

    private Task HandleErrorAsync(Exception exception)
    {
        return Task.CompletedTask;
    }

    private Task HandleCompletedAsync()
    {
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(SnackInitializedEvent snackEvent)
    {
        if (snackEvent.SnackId != Id)
        {
            return Task.CompletedTask;
        }
        var dispatcher = Application.Current.Dispatcher;
        dispatcher?.Invoke(() =>
                           {
                               Name = snackEvent.Name;
                               PictureUrl = snackEvent.PictureUrl;
                           });
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(SnackDeletedEvent snackEvent)
    {
        if (snackEvent.SnackId != Id)
        {
            return Task.CompletedTask;
        }
        var dispatcher = Application.Current.Dispatcher;
        dispatcher?.Invoke(() =>
                           {
                               IsDeleted = true;
                           });
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(SnackUpdatedEvent snackEvent)
    {
        if (snackEvent.SnackId != Id)
        {
            return Task.CompletedTask;
        }
        var dispatcher = Application.Current.Dispatcher;
        dispatcher?.Invoke(() =>
                           {
                               Name = snackEvent.Name;
                               PictureUrl = snackEvent.PictureUrl;
                           });
        return Task.CompletedTask;
    }

    private Task ApplyErrorEventAsync(SnackErrorEvent errorEvent)
    {
        return Task.CompletedTask;
    }

    #endregion

}