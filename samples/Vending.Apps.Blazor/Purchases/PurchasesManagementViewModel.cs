using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;
using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using Orleans.FluentResults;
using Orleans.Streams;
using ReactiveUI;
using SiloX.Domain.Abstractions;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Apps.Blazor.Services;
using Vending.Domain.Abstractions;
using Vending.Projection.Abstractions.Purchases;

namespace Vending.Apps.Blazor.Purchases;

public class PurchasesManagementViewModel : ReactiveObject, IActivatableViewModel
{
    private StreamSequenceToken? _purchaseSequenceToken;

    #region Constructor

    /// <inheritdoc />
    public PurchasesManagementViewModel(IClusterClientReady clusterClientReady)
    {
        // When the cluster client is ready, set the cluster client.
        ClusterClientReady = Guard.Against.Null(clusterClientReady, nameof(clusterClientReady));
        this.WhenAnyValue(vm => vm.ClusterClientReady)
            .Where(clientReady => clientReady != null)
            .SelectMany(clientReady => clientReady!.ClusterClient.Task)
            .Subscribe(client => ClusterClient = client);

        // Create the cache for the purchases.
        var purchasesCache = new SourceCache<PurchaseViewModel, Guid>(purchase => purchase.Id);
        var purchasesObs = purchasesCache.Connect()
                                         .Filter(this.WhenAnyValue(vm => vm.BoughtPriceStart, vm => vm.BoughtPriceEnd, vm => vm.SearchTerm)
                                                     .Throttle(TimeSpan.FromMilliseconds(500))
                                                     .DistinctUntilChanged()
                                                     .Select(tuple => new Func<PurchaseViewModel, bool>(purchase => (tuple.Item1 == null || purchase.BoughtPrice >= tuple.Item1)
                                                                                                                 && (tuple.Item2 == null || purchase.BoughtPrice < tuple.Item2)
                                                                                                                 && (tuple.Item3.IsNullOrEmpty() || purchase.SnackName.Contains(tuple.Item3, StringComparison.OrdinalIgnoreCase)))))
                                         .Publish()
                                         .RefCount();
        // Sort and page the purchases.
        purchasesObs.Sort(SortExpressionComparer<PurchaseViewModel>.Descending(purchase => purchase.BoughtAt))
                    .Page(this.WhenAnyValue(vm => vm.PageNumber, vm => vm.PageSize)
                              .DistinctUntilChanged()
                              .Select(tuple => new PageRequest(tuple.Item1, tuple.Item2)))
                    .Bind(out var purchases)
                    .Subscribe(set => PurchasesChangeSet = set);
        Purchases = purchases;

        // Recalculate the page count when the cache changes.
        purchasesObs.Count()
                    .Subscribe(count =>
                               {
                                   PurchaseCount = count;
                                   PageCount = (int)Math.Ceiling((double)count / PageSize);
                               });
        this.WhenAnyValue(vm => vm.PageSize)
            .Subscribe(size => PageCount = (int)Math.Ceiling((double)purchasesCache.Count / size));

        // Recalculate the page number when the page size or page count changes.
        this.WhenAnyValue(vm => vm.PageSize, vm => vm.PageCount)
            .Subscribe(tuple =>
                       {
                           var pageNumber = PageNumber <= 0 ? 1 : PageNumber;
                           var oldOffset = _oldPageSize * (pageNumber - 1) + 1;
                           var newPageNumber = (int)Math.Ceiling((double)oldOffset / tuple.Item1);
                           pageNumber = Math.Min(newPageNumber, tuple.Item2);
                           PageNumber = pageNumber <= 0 ? 1 : pageNumber;
                       });

        // Get purchases and update the cache.
        this.WhenAnyValue(vm => vm.BoughtPriceStart, vm => vm.BoughtPriceEnd, vm => vm.SearchTerm, vm => vm.ClusterClient)
            .Where(tuple => tuple.Item4 != null)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .DistinctUntilChanged()
            .Select(tuple => (tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4!.GetGrain<IPurchaseRetrieverGrain>("Manager")))
            .SelectMany(tuple => tuple.Item4.SearchingPagedListAsync(new PurchaseRetrieverSearchingPagedListQuery(tuple.Item3, 0, 100, new Dictionary<string, bool> { { "BoughtAt", true } }, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager", null, null,
                                                                                                                  new DecimalRange(tuple.Item1, tuple.Item2))))
            .Where(result => result.IsSuccess)
            .Select(result => result.Value)
            .Subscribe(purchasesList => purchasesCache.AddOrUpdateWith(purchasesList));

        // Stream events subscription.
        this.WhenActivated(disposable =>
                           {
                               // When the cluster client changes, subscribe to the purchase info stream.
                               var allPurchasesStreamObs = this.WhenAnyValue(vm => vm.ClusterClient)
                                                               .DistinctUntilChanged()
                                                               .Where(client => client != null)
                                                               .SelectMany(client => client!.GetReceiverStreamWithGuidKey<PurchaseInfoEvent>(Constants.StreamProviderName, Constants.PurchaseInfosBroadcastNamespace, _purchaseSequenceToken))
                                                               .Publish()
                                                               .RefCount();
                               allPurchasesStreamObs.Where(tuple => tuple.Event is PurchaseInfoSavedEvent)
                                                    .Subscribe(tuple =>
                                                               {
                                                                   _purchaseSequenceToken = tuple.SequenceToken;
                                                                   var savedEvent = (PurchaseInfoSavedEvent)tuple.Event;
                                                                   purchasesCache.AddOrUpdateWith(savedEvent.Purchase);
                                                               })
                                                    .DisposeWith(disposable);
                               allPurchasesStreamObs.Where(tuple => tuple.Event is PurchaseInfoErrorEvent)
                                                    .Subscribe(tuple =>
                                                               {
                                                                   var errorEvent = (PurchaseInfoErrorEvent)tuple.Event;
                                                                   ErrorInfo = $"{errorEvent.Code}:{string.Join("\n", errorEvent.Reasons)}";
                                                               })
                                                    .DisposeWith(disposable);
                           });

        // Create the commands.
        SyncPurchasesCommand = ReactiveCommand.CreateFromTask(SyncPurchasesAsync, CanSyncPurchases);
        GoPreviousPageCommand = ReactiveCommand.Create(GoPreviousPage, CanGoPreviousPage);
        GoNextPageCommand = ReactiveCommand.Create(GoNextPage, CanGoNextPage);
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

    private int _pageSize = 10;
    private int _oldPageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set
        {
            _oldPageSize = _pageSize;
            this.RaiseAndSetIfChanged(ref _pageSize, value < 1 ? 1 : value);
        }
    }

    private int _pageCount;
    public int PageCount
    {
        get => _pageCount;
        set => this.RaiseAndSetIfChanged(ref _pageCount, value < 1 ? 1 : value);
    }

    private int _pageNumber = 1;
    public int PageNumber
    {
        get => _pageNumber;
        set => this.RaiseAndSetIfChanged(ref _pageNumber, value < 1 ? 1 : value);
    }

    private string _searchTerm = string.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
    }

    private decimal? _boughtPriceStart;
    public decimal? BoughtPriceStart
    {
        get => _boughtPriceStart;
        set => this.RaiseAndSetIfChanged(ref _boughtPriceStart, value);
    }

    private decimal? _boughtPriceEnd;
    public decimal? BoughtPriceEnd
    {
        get => _boughtPriceEnd;
        set => this.RaiseAndSetIfChanged(ref _boughtPriceEnd, value);
    }

    public ReadOnlyObservableCollection<PurchaseViewModel> Purchases { get; }

    private int _purchaseCount;
    public int PurchaseCount
    {
        get => _purchaseCount;
        set => this.RaiseAndSetIfChanged(ref _purchaseCount, value);
    }

    private IChangeSet<PurchaseViewModel, Guid>? _purchasesChangeSet;
    public IChangeSet<PurchaseViewModel, Guid>? PurchasesChangeSet
    {
        get => _purchasesChangeSet;
        set => this.RaiseAndSetIfChanged(ref _purchasesChangeSet, value);
    }

    #endregion

    #region Interactions

    /// <summary>
    ///     Interaction for errors.
    /// </summary>
    public Interaction<IEnumerable<IError>, ErrorRecovery> ErrorsInteraction { get; } = new();

    #endregion

    #region Commands

    /// <summary>
    ///     Gets the command that syncs the purchase data.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SyncPurchasesCommand { get; }

    /// <summary>
    ///     Gets the observable that indicates whether the sync purchases command can be executed.
    /// </summary>
    private IObservable<bool> CanSyncPurchases =>
        this.WhenAnyValue(vm => vm.ClusterClient)
            .Select(client => client != null);

    /// <summary>
    ///     Syncs the purchase data.
    /// </summary>
    private async Task SyncPurchasesAsync()
    {
        bool retry;
        do
        {
            var result = await Result.Ok()
                                     .Ensure(ClusterClient != null, "No cluster client available.")
                                     .MapTry(() => ClusterClient!.GetGrain<IPurchaseSynchronizerGrain>(string.Empty))
                                     .MapTryAsync(grain => grain.SyncDifferencesAsync());
            if (result.IsSuccess)
            {
                return;
            }
            var errorRecovery = await ErrorsInteraction.Handle(result.Errors);
            retry = errorRecovery == ErrorRecovery.Retry;
        }
        while (retry);
    }

    /// <summary>
    ///     Gets the command that moves to the previous page.
    /// </summary>
    public ReactiveCommand<Unit, Unit> GoPreviousPageCommand { get; }

    private IObservable<bool> CanGoPreviousPage =>
        this.WhenAnyValue(vm => vm.PageNumber)
            .Select(number => number > 1);

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
            .Select(tuple => tuple.Item1 < tuple.Item2);

    /// <summary>
    ///     Moves to the next page.
    /// </summary>
    private void GoNextPage()
    {
        PageNumber++;
    }

    #endregion

}