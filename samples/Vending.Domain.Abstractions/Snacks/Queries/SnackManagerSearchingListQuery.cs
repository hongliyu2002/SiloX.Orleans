using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Snacks;

/// <summary>
///     A class that represents a query for a snacks with searching feature.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackManagerSearchingListQuery
    (string? SearchCriteria,
     DateTimeOffsetRange? CreatedAtRange,
     string? CreatedBy,
     DateTimeOffsetRange? LastModifiedAtRange,
     string? LastModifiedBy,
     DateTimeOffsetRange? DeletedAtRange,
     string? DeletedBy,
     bool? IsDeleted,
     IDictionary<string, bool>? Sortings,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainSearchingListQuery(SearchCriteria, Sortings, TraceId, OperatedAt, OperatedBy);
