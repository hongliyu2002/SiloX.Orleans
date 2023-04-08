using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Purchases;

/// <summary>
///     A query for a list of purchases.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseRetrieverListQuery
    (IDictionary<string, bool>? OrderBy,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy,
     Guid? MachineId = null,
     Guid? SnackId = null,
     DecimalRange? BoughtPriceRange = null,
     DateTimeOffsetRange? BoughtAtRange = null,
     string? BoughtBy = null) : DomainListQuery(OrderBy, TraceId, OperatedAt, OperatedBy);