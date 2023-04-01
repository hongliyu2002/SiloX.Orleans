using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Purchases;

/// <summary>
///     A query for a list of purchases.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseProjectionManagerListQuery
    (Guid? MachineId,
     Guid? SnackId,
     DecimalRange? BoughtPriceRange,
     DateTimeOffsetRange? BoughtAtRange,
     string? BoughtBy,
     IDictionary<string, bool>? Sortings,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainListQuery(Sortings, TraceId, OperatedAt, OperatedBy);
