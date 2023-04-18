using System.Reactive.Disposables;
using System.Reactive.Linq;
using Fluxera.Guards;
using Orleans.Streams;
using ReactiveUI;
using SiloX.Domain.Abstractions.Extensions;
using Splat;
using Vending.Apps.Blazor.Services;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Snacks;

namespace Vending.Apps.Blazor.Business;

public class SnackViewModel : ReactiveObject, IActivatableViewModel
{
    private StreamSequenceToken? _lastSequenceToken;

    #region Constructor

    public SnackViewModel(Guid snackId)
    {
        Id = Guard.Against.Empty(snackId, nameof(snackId));

        // When the cluster client is ready, set the cluster client.
        ClusterClientReady = Locator.Current.GetService<IClusterClientReady>();
        this.WhenAnyValue(vm => vm.ClusterClientReady)
            .Where(clientReady => clientReady != null)
            .SelectMany(clientReady => clientReady!.ClusterClient.Task)
            .Subscribe(client => ClusterClient = client);

        // Load the snack and update the view model.
        this.WhenAnyValue(vm => vm.Id, vm => vm.ClusterClient)
            .Where(tuple => tuple.Item1 != Guid.Empty && tuple.Item2 != null)
            .DistinctUntilChanged()
            .Select(tuple => tuple.Item2!.GetGrain<ISnackGrain>(tuple.Item1))
            .SelectMany(grain => grain.GetSnackAsync())
            .Subscribe(UpdateWith);

        // Subscribe to the snack stream.
        this.WhenActivated(disposable =>
                           {
                               // When the cluster client changes, subscribe to the snack info stream.
                               var streamObs = this.WhenAnyValue(vm => vm.Id, vm => vm.ClusterClient)
                                                   .Where(tuple => tuple.Item1 != Guid.Empty && tuple.Item2 != null)
                                                   .DistinctUntilChanged()
                                                   .SelectMany(tuple => tuple.Item2!.GetSubscriberStreamWithGuidKey<SnackEvent>(Constants.StreamProviderName, Constants.SnacksNamespace, tuple.Item1, _lastSequenceToken))
                                                   .Publish()
                                                   .RefCount();
                               streamObs.Where(tuple => tuple.Event is SnackInitializedEvent)
                                        .Subscribe(tuple =>
                                                   {
                                                       _lastSequenceToken = tuple.SequenceToken;
                                                       var snackEvent = (SnackInitializedEvent)tuple.Event;
                                                       Name = snackEvent.Name;
                                                       PictureUrl = snackEvent.PictureUrl;
                                                   })
                                        .DisposeWith(disposable);
                               streamObs.Where(tuple => tuple.Event is SnackDeletedEvent)
                                        .Subscribe(tuple =>
                                                   {
                                                       _lastSequenceToken = tuple.SequenceToken;
                                                       IsDeleted = true;
                                                   })
                                        .DisposeWith(disposable);
                               streamObs.Where(tuple => tuple.Event is SnackUpdatedEvent)
                                        .Subscribe(tuple =>
                                                   {
                                                       _lastSequenceToken = tuple.SequenceToken;
                                                       var snackEvent = (SnackUpdatedEvent)tuple.Event;
                                                       Name = snackEvent.Name;
                                                       PictureUrl = snackEvent.PictureUrl;
                                                   })
                                        .DisposeWith(disposable);
                               streamObs.Where(tuple => tuple.Event is SnackErrorEvent)
                                        .Subscribe(tuple =>
                                                   {
                                                       var errorEvent = (SnackErrorEvent)tuple.Event;
                                                       ErrorInfo = $"{errorEvent.Code}:{string.Join("\n", errorEvent.Reasons)}";
                                                   })
                                        .DisposeWith(disposable);
                           });
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    public ViewModelActivator Activator { get; } = new();

    private readonly IClusterClientReady? _clusterClientReady;
    public IClusterClientReady? ClusterClientReady
    {
        get => _clusterClientReady;
        init => this.RaiseAndSetIfChanged(ref _clusterClientReady, value);
    }

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

    #region Load Snack

    public void UpdateWith(Snack snack)
    {
        Id = snack.Id;
        Name = snack.Name;
        PictureUrl = snack.PictureUrl;
        IsDeleted = snack.IsDeleted;
    }

    #endregion

}