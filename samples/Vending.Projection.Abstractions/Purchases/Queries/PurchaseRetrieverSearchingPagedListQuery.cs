using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Purchases;

/// <summary>
///     A query for retrieving a paged list of purchases with searching feature.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseRetrieverSearchingPagedListQuery
    (string? SearchTerm,
     int? SkipCount,
     int? MaxResultCount,
     IDictionary<string, bool>? OrderBy,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy,
     Guid? MachineId = null,
     Guid? SnackId = null,
     DecimalRange? BoughtPriceRange = null,
     DateTimeOffsetRange? BoughtAtRange = null,
     string? BoughtBy = null) : DomainSearchingPagedListQuery(SearchTerm, SkipCount, MaxResultCount, OrderBy, TraceId, OperatedAt, OperatedBy);