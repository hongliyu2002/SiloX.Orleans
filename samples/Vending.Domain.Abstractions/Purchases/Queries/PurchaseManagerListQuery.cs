using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Purchases;

/// <summary>
///     A query for a list of snack machines.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseManagerListQuery
    (Guid? MachineId,
     Guid? SnackId,
     DecimalRange? BoughtPriceRange,
     DateTimeOffsetRange? BoughtAtRange,
     string? BoughtBy,
     IDictionary<string, bool>? Sortings,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainListQuery(Sortings, TraceId, OperatedAt, OperatedBy);
