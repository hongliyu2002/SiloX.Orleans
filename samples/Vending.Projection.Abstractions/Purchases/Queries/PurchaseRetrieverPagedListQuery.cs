using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Purchases;

/// <summary>
///     A query for retrieving a paged list of purchases.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseRetrieverPagedListQuery
    (Guid? MachineId,
     Guid? SnackId,
     DecimalRange? BoughtPriceRange,
     DateTimeOffsetRange? BoughtAtRange,
     string? BoughtBy,
     int? SkipCount,
     int? MaxResultCount,
     IDictionary<string, bool>? Sortings,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainPagedListQuery(SkipCount, MaxResultCount, Sortings, TraceId, OperatedAt, OperatedBy);
