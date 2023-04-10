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

namespace Vending.App.Snacks;

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
                                   .Where(idClient => idClient.Item1 != Guid.Empty && idClient.Item2 != null)
                                   .Select(idClient => (idClient.Item1, idClient.Item2!.GetStreamProvider(Constants.StreamProviderName)))
                                   .Select(idProvider => idProvider.Item2.GetStream<SnackEvent>(StreamId.Create(Constants.SnacksNamespace, idProvider.Item1)))
                                   .SelectMany(stream => stream.SubscribeAsync(HandleEventAsync, HandleErrorAsync, HandleCompletedAsync, _lastSequenceToken))
                                   .ObserveOn(RxApp.MainThreadScheduler)
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
        switch (domainEvent)
        {
            case SnackInitializedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case SnackDeletedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case SnackUpdatedEvent snackEvent:
                return ApplyEventAsync(snackEvent);
            case SnackErrorEvent snackEvent:
                return ApplyErrorEventAsync(snackEvent);
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

    private Task ApplyEventAsync(SnackInitializedEvent snackEvent)
    {
        if (snackEvent.SnackId == Id)
        {
            Name = snackEvent.Name;
            PictureUrl = snackEvent.PictureUrl;
        }
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(SnackDeletedEvent snackEvent)
    {
        if (snackEvent.SnackId == Id)
        {
            IsDeleted = true;
        }
        return Task.CompletedTask;
    }

    private Task ApplyEventAsync(SnackUpdatedEvent snackEvent)
    {
        if (snackEvent.SnackId == Id)
        {
            Name = snackEvent.Name;
            PictureUrl = snackEvent.PictureUrl;
        }
        return Task.CompletedTask;
    }

    private Task ApplyErrorEventAsync(SnackErrorEvent errorEvent)
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

    private Guid _id;

    public Guid Id
    {
        get => _id;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _id, value);
                               });
        }
    }

    private string _name = string.Empty;

    public string Name
    {
        get => _name;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _name, value);
                               });
        }
    }

    private string? _pictureUrl;

    public string? PictureUrl
    {
        get => _pictureUrl;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _pictureUrl, value);
                               });
        }
    }

    private bool _isDeleted;

    public bool IsDeleted
    {
        get => _isDeleted;
        set
        {
            var dispatcher = Application.Current.Dispatcher;
            dispatcher?.Invoke(() =>
                               {
                                   this.RaiseAndSetIfChanged(ref _isDeleted, value);
                               });
        }
    }

    #endregion

    #region Commands

    /// <summary>
    ///     Gets the command to save the snack.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SaveSnackCommand { get; }

    private IObservable<bool> CanSaveSnack =>
        this.WhenAnyValue(vm => vm.Name, vm => vm.IsDeleted, vm => vm.ClusterClient)
            .Select(nameClient => nameClient.Item1.IsNotNullOrEmpty() && nameClient is { Item2: false, Item3: not null });

    private async Task SaveSnackAsync()
    {
        bool retry;
        do
        {
            ISnackRepoGrain repoGrain = null!;
            var result = await Result.Ok()
                                     .MapTry(() => repoGrain = ClusterClient!.GetGrain<ISnackRepoGrain>("Manager"))
                                     .BindTryIfAsync(Id == Guid.Empty, () => repoGrain.CreateAsync(new SnackRepoCreateCommand(Name, PictureUrl, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
                                     .BindTryIfAsync<Snack>(Id != Guid.Empty, () => repoGrain.UpdateAsync(new SnackRepoUpdateCommand(Id, Name, PictureUrl, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
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

}