using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Purchases;

/// <summary>
///     A query for retrieving a paged list of purchases with searching feature.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseProjectionManagerSearchingPagedListQuery
    (string? SearchCriteria,
     Guid? MachineId,
     Guid? SnackId,
     DecimalRange? BoughtPriceRange,
     DateTimeOffsetRange? BoughtAtRange,
     string? BoughtBy,
     int? SkipCount,
     int? MaxResultCount,
     IDictionary<string, bool>? Sortings,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainSearchingPagedListQuery(SearchCriteria, SkipCount, MaxResultCount, Sortings, TraceId, OperatedAt, OperatedBy);
