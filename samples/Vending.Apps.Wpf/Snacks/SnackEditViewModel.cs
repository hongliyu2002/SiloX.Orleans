using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using Orleans.FluentResults;
using Orleans.Streams;
using ReactiveUI;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Snacks;

namespace Vending.App.Wpf.Snacks;

public class SnackEditViewModel : ReactiveObject, IActivatableViewModel
{
    private StreamSequenceToken? _lastSequenceToken;

    public SnackEditViewModel(Snack snack, IClusterClient clusterClient)
    {
        Guard.Against.Null(snack, nameof(snack));
        ClusterClient = Guard.Against.Null(clusterClient, nameof(clusterClient));
        this.WhenActivated(disposable =>
                           {
                               // When the cluster client changes, subscribe to the snack info stream.
                               var streamObs = this.WhenAnyValue(vm => vm.Id, vm => vm.ClusterClient)
                                                   .Where(tuple => tuple.Item1 != Guid.Empty && tuple.Item2 != null)
                                                   .SelectMany(tuple => tuple.Item2!.GetSubscriberStreamWithGuidKey<SnackEvent>(Constants.StreamProviderName, Constants.SnacksNamespace, tuple.Item1, _lastSequenceToken))
                                                   .Publish()
                                                   .RefCount();
                               streamObs.Where(tuple => tuple.Event is SnackInitializedEvent)
                                        .ObserveOn(RxApp.MainThreadScheduler)
                                        .Subscribe(tuple =>
                                                   {
                                                       _lastSequenceToken = tuple.SequenceToken;
                                                       var snackEvent = (SnackInitializedEvent)tuple.Event;
                                                       Name = snackEvent.Name;
                                                       PictureUrl = snackEvent.PictureUrl;
                                                   })
                                        .DisposeWith(disposable);
                               streamObs.Where(tuple => tuple.Event is SnackDeletedEvent)
                                        .ObserveOn(RxApp.MainThreadScheduler)
                                        .Subscribe(tuple =>
                                                   {
                                                       _lastSequenceToken = tuple.SequenceToken;
                                                       IsDeleted = true;
                                                   })
                                        .DisposeWith(disposable);
                               streamObs.Where(tuple => tuple.Event is SnackUpdatedEvent)
                                        .ObserveOn(RxApp.MainThreadScheduler)
                                        .Subscribe(tuple =>
                                                   {
                                                       _lastSequenceToken = tuple.SequenceToken;
                                                       var snackEvent = (SnackUpdatedEvent)tuple.Event;
                                                       Name = snackEvent.Name;
                                                       PictureUrl = snackEvent.PictureUrl;
                                                   })
                                        .DisposeWith(disposable);
                               streamObs.Where(tuple => tuple.Event is SnackErrorEvent)
                                        .ObserveOn(RxApp.MainThreadScheduler)
                                        .Subscribe(tuple =>
                                                   {
                                                       var errorEvent = (SnackErrorEvent)tuple.Event;
                                                       ErrorInfo = $"{errorEvent.Code}:{string.Join("\n", errorEvent.Reasons)}";
                                                   })
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

    private string _errorInfo = string.Empty;
    public string ErrorInfo
    {
        get => _errorInfo;
        set => this.RaiseAndSetIfChanged(ref _errorInfo, value);
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

}