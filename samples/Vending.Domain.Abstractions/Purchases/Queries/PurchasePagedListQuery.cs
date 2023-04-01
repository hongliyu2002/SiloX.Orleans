using System.Collections.Immutable;
using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Purchases;

/// <summary>
///     This class represents a query for retrieving a paged list of snack machines.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchasePagedListQuery
    (Guid? MachineId,
     Guid? SnackId,
     DecimalRange? BoughtPrice,
     DateTimeOffsetRange? BoughtAtRange,
     string? BoughtBy,
     int? SkipCount,
     int? MaxResultCount,
     IImmutableList<KeyValuePair<string, bool>>? Sortings,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainPagedListQuery(SkipCount, MaxResultCount, Sortings, TraceId, OperatedAt, OperatedBy);
